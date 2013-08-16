//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Sherlock.Service.Master.Properties;
using Sherlock.Shared.Core;

namespace Sherlock.Service.Master
{
    /// <summary>
    /// Provides helper functions for dealing with machines, either virtual or physical.
    /// </summary>
    internal static class MachineHelpers
    {
        /// <summary>
        /// Returns a value indicating if the given machine is alive and can be reached via a network connection of sorts.
        /// </summary>
        /// <param name="networkName">The network name of the machine.</param>
        /// <param name="pingTimeoutInMilliSeconds">The number of milliseconds that the ping operation should wait for a response.</param>
        /// <returns>
        /// <see langword="true" /> if the machine could be reached; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool IsMachineAlive(string networkName, int pingTimeoutInMilliSeconds)
        {
            try
            {
                using (var ping = new Ping())
                {
                    // The ping request can fail if there is no machine with the given
                    // network name ...
                    var reply = ping.Send(networkName, pingTimeoutInMilliSeconds);
                    if (reply.Status != IPStatus.Success)
                    {
                        return false;
                    }
                }
            }
            catch (PingException e)
            {
                // Machine is unknown
                throw new MachineUnreachableException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_MachineUnreachable_WithNetworkName,
                        networkName),
                    e);
            }

            return true;
        }

        /// <summary>
        /// Tries to wake a physical machine by sending the Wake-On-LAN magic packet to the machine with the given MAC address.
        /// </summary>
        /// <param name="macAddress">The MAC address of the machine.</param>
        private static void WakeMachine(string macAddress)
        {
            // Use the Wake-on-Lan approach to wake the machine up
            // See here: http://blog.memos.cz/name.php/team/2008/06/12/wake-on-lan-in-csharp
            // and here: http://www.stev.org/post/2011/02/10/C-Wake-on-Lan-%28WOL%29-Packet.aspx
            //
            // total number of bytes should be 102
            var buffer = new byte[102];

            // first 6 bytes should be 0xFF
            int offset = 0;
            for (int i = 0; i < 6; i++)
            {
                buffer[offset++] = 0xFF;
            }

            // now repeat MAC 16 times
            for (int i = 0; i < 16; i++)
            {
                int k = 0;
                for (int j = 0; j < 6; j++)
                {
                    buffer[offset++] = byte.Parse(macAddress.Substring(k, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    k += 2;
                }
            }

            try
            {
                using (var udp = new UdpClient())
                {
                    udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    udp.EnableBroadcast = true;
                    udp.Send(buffer, 102, new IPEndPoint(IPAddress.Parse("255.255.255.255"), 0x1));
                }
            }
            catch (SocketException)
            {
                // For now just ignore it
            }
        }

        /// <summary>
        /// Waits for a given machine to connect to the network and respond to a network ping message.
        /// </summary>
        /// <param name="networkName">The network name of the machine.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="pingTimeoutInMilliSeconds">The number of milliseconds that the ping operation should wait for a response.</param>
        /// <param name="maximumWaitTimeInMilliSeconds">
        ///     The number of milliseconds that the operation should wait for the machine to come online.
        /// </param>
        /// <param name="pingCycleTimeInMilliSeconds">The amount of time the operation should wait between consecutive ping operations.</param>
        /// <returns>
        ///     <see langword="true" /> if the machine could be reached within the <paramref name="maximumWaitTimeInMilliSeconds"/>; 
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        private static bool AllowTimeForMachineToConnectToNetwork(
            string networkName, 
            SystemDiagnostics diagnostics,
            int pingTimeoutInMilliSeconds, 
            int maximumWaitTimeInMilliSeconds,
            int pingCycleTimeInMilliSeconds)
        {
            return WaitForResponse(
                () => IsMachineAlive(networkName, pingTimeoutInMilliSeconds),
                diagnostics,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} connected to network",
                    networkName),
                maximumWaitTimeInMilliSeconds, 
                pingCycleTimeInMilliSeconds);
        }

        /// <summary>
        /// Waits for a given action to happen within a given timeout time.
        /// </summary>
        /// <param name="hasHadResponse">The function which determines if the desired response has been received yet.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="eventDescription">The description for the event which the application is waiting for.</param>
        /// <param name="maximumWaitTimeInMilliSeconds">
        ///     The number of milliseconds that the operation should wait for the machine to come online.
        /// </param>
        /// <param name="cycleTimeInMilliSeconds">The amount of time the operation should wait between consecutive operations.</param>
        /// <returns>
        ///     <see langword="true" /> if the action has been executed within the <paramref name="maximumWaitTimeInMilliSeconds"/>; 
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool WaitForResponse(
            Func<bool> hasHadResponse,
            SystemDiagnostics diagnostics,
            string eventDescription,
            int maximumWaitTimeInMilliSeconds,
            int cycleTimeInMilliSeconds)
        {
            var maximumWaitTime = TimeSpan.FromMilliseconds(maximumWaitTimeInMilliSeconds);
            var killTime = DateTimeOffset.Now + maximumWaitTime;
            while (!hasHadResponse())
            {
                diagnostics.Log(
                    LevelToLog.Trace,
                    MachineConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Waiting for event: {0}",
                        eventDescription));

                Thread.Sleep(cycleTimeInMilliSeconds);
                if (DateTimeOffset.Now > killTime)
                {
                    diagnostics.Log(
                    LevelToLog.Trace,
                    MachineConstants.LogPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Event [{0}] did not occur within the designated time window of {1}",
                        eventDescription,
                        maximumWaitTime));

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a networked machine is alive on the network and if not will try to 
        /// wake it up from sleep and wait for it to sign in to the network.
        /// </summary>
        /// <param name="networkName">The network name of the machine.</param>
        /// <param name="macAddress">The MAC address of the machine.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="canStartRemotely">A flag indicating if the machine can be woken up or not.</param>
        /// <param name="pingTimeoutInMilliSeconds">The number of milliseconds that the ping operation should wait for a response.</param>
        /// <param name="maximumWaitTimeInMilliSeconds">
        ///     The number of milliseconds that the operation should wait for the machine to come online.
        /// </param>
        /// <param name="pingCycleTimeInMilliSeconds">The amount of time the operation should wait between consecutive ping operations.</param>
        /// <returns>
        ///     <see langword="true" /> if the machine could be reached within the <paramref name="maximumWaitTimeInMilliSeconds"/>; 
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool WakeNetworkedMachineAndWaitForNetworkSignIn(
            string networkName,
            string macAddress,
            SystemDiagnostics diagnostics,
            bool canStartRemotely,
            int pingTimeoutInMilliSeconds, 
            int maximumWaitTimeInMilliSeconds,
            int pingCycleTimeInMilliSeconds)
        {
            if (!IsMachineAlive(networkName, pingTimeoutInMilliSeconds))
            {
                if (!canStartRemotely)
                {
                    return false;
                }

                WakeMachine(macAddress);
                if (!AllowTimeForMachineToConnectToNetwork(
                    networkName, 
                    diagnostics,
                    pingTimeoutInMilliSeconds, 
                    maximumWaitTimeInMilliSeconds, 
                    pingCycleTimeInMilliSeconds))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

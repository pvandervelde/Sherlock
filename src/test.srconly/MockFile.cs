//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;

namespace Test.Mocks
{
    internal sealed class MockFile : FileBase
    {
        private readonly Dictionary<string, string> m_Content
            = new Dictionary<string, string>();

        public MockFile(string path, string content)
        {
            m_Content.Add(path, content);
        }

        public MockFile(Dictionary<string, string> files)
        {
            m_Content = files;
        }

        public override void AppendAllText(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override StreamWriter AppendText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Copy(string sourceFileName, string destFileName)
        {
            // Do nothing for now
        }

        public override void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            // Do nothing for now
        }

        public override Stream Create(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            throw new NotImplementedException();
        }

        public override StreamWriter CreateText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Decrypt(string path)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string path)
        {
            throw new NotImplementedException();
        }

        public override void Encrypt(string path)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override FileAttributes GetAttributes(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastAccessTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastAccessTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastWriteTime(string path)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public override void Move(string sourceFileName, string destFileName)
        {
            throw new NotImplementedException();
        }

        public override Stream Open(string path, FileMode mode)
        {
            return Open(path, mode, FileAccess.ReadWrite);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access)
        {
            return Open(path, mode, access, FileShare.ReadWrite);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            var output = new MemoryStream();
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(m_Content[path]);
                writer.Flush();

                stream.Position = 0;
                stream.CopyTo(output);
                output.Position = 0;
            }

            return output;
        }

        public override Stream OpenRead(string path)
        {
            throw new NotImplementedException();
        }

        public override StreamReader OpenText(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        public override byte[] ReadAllBytes(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            throw new NotImplementedException();
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            throw new NotImplementedException();
        }

        public override void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            throw new NotImplementedException();
        }

        public override void SetAttributes(string path, FileAttributes fileAttributes)
        {
            throw new NotImplementedException();
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            throw new NotImplementedException();
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            throw new NotImplementedException();
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            throw new NotImplementedException();
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            throw new NotImplementedException();
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            throw new NotImplementedException();
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, string[] contents)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllText(string path, string contents)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllText(string path, string contents, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}

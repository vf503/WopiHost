﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WopiCobaltHost
{
    public class FileSession : EditSession
    {
        public FileSession(string sessionId, string filePath, string login = "Anonymous", string name = "Anonymous", string email = "", bool isAnonymous = true)
            :base(sessionId, filePath, login, name, email, isAnonymous)
        { }

        override public WopiCheckFileInfo GetCheckFileInfo()
        {
            WopiCheckFileInfo cfi = new WopiCheckFileInfo();

            cfi.BaseFileName = m_fileinfo.Name;
            cfi.OwnerId = m_login;
            cfi.UserFriendlyName = m_name;

            lock (m_fileinfo)
            {
                if (m_fileinfo.Exists)
                {
                    cfi.Size = m_fileinfo.Length;
                }
                else
                {
                    cfi.Size = 0;
                }
            }

            cfi.Version = DateTime.Now.ToString("s");
            cfi.SupportsCoauth = false;
            cfi.SupportsCobalt = false;
            cfi.SupportsFolders = true;
            cfi.SupportsLocks = true;
            cfi.SupportsScenarioLinks = false;
            cfi.SupportsSecureStore = false;
            cfi.SupportsUpdate = true;
            cfi.UserCanWrite = true;

            //
            var hasher = System.Security.Cryptography.SHA256.Create();
            byte[] hashValue;
            using (Stream s = m_fileinfo.OpenRead())
            {
                hashValue = hasher.ComputeHash(s);
            }
            string sha256 = Convert.ToBase64String(hashValue);
            cfi.SHA256 = sha256;
            //
            return cfi;
        }
        override public byte[] GetFileContent()
        {
            MemoryStream ms = new MemoryStream();
            lock (m_fileinfo)
            {
                using (FileStream fileStream = m_fileinfo.OpenRead())
                {
                    fileStream.CopyTo(ms);
                }
            }
            return ms.ToArray();
        }
        override public void Save(byte[] new_content)
        {
            lock (m_fileinfo)
            {
                if (m_fileinfo.Exists)
                {
                    using (FileStream fileStream = m_fileinfo.Open(FileMode.Truncate))
                    {
                        fileStream.Write(new_content, 0, new_content.Length);
                    }
                }
                else
                {
                    using (FileStream fileStream = m_fileinfo.Open(FileMode.Create))
                    {
                        fileStream.Write(new_content, 0, new_content.Length);
                    }
                }
            }
            m_lastUpdated = DateTime.Now;
        }
    }
}

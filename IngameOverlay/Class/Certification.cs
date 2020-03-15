using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Resources;
using static System.Net.Mime.MediaTypeNames;

namespace IngameOverlay
{
    class Certification
    {
        public static void Install(byte[] cer)
        {
            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadWrite);
            var cert = new X509Certificate2(cer);
            store.Add(cert);
            store.Close();
        }
    }
}

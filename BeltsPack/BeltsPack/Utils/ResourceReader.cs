using System;
using System.IO;

namespace BeltsPack.Utils
{
    class ResourceReader
    {
        // convert pdf into binary
        public byte[] PdfToBinary(string path)
        {
            // read the file
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            // Convert the pdf into binary array
            BinaryReader br = new BinaryReader(fs);
            byte[] BinaryArray = br.ReadBytes((Int32)fs.Length);

            // output
            return BinaryArray;
        }


        // convert image to binary
        public byte[] ImageToBinary(System.Drawing.Image img)
        {

            // Convert the pdf into binary array
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }

        }
    }
}

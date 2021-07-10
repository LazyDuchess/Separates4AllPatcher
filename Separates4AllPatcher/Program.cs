using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Separates4AllPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static bool ReplaceBytes(byte[] fileBytes, byte[] replaceWith, int position)
        {
            for(var i=0;i<replaceWith.Length;i++)
            {
                fileBytes[position + i] = replaceWith[i];
            }
            return true;
        }

        //thanks https://social.msdn.microsoft.com/Forums/vstudio/en-US/15514c1a-b6a1-44f5-a06c-9b029c4164d7/searching-a-byte-array-for-a-pattern-of-bytes?forum=csharpgeneral
        public static int IndexOf(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int IndexOfCodeCave(byte[] arrayToSearchThrough, int length)
        {
            int curByte = 0;
            byte[] caveBytes = new byte[] { 0xCC };
            var found = -1;
            var codeCaveLookup = new byte[length];
            while (found == -1 && curByte < caveBytes.Length)
            {
                for (var i = 0; i < codeCaveLookup.Length; i++)
                {
                    codeCaveLookup[i] = caveBytes[curByte];
                }
                found = IndexOf(arrayToSearchThrough, codeCaveLookup);
                curByte += 1;
            }
            return found;
        }

        public static void PatchExe(string exeFile)
        {
            var bakpath = Path.Combine(Path.GetDirectoryName(exeFile), Path.GetFileNameWithoutExtension(exeFile) + ".bak");
            if (!File.Exists(bakpath))
            {
                try
                {
                    File.Copy(exeFile, bakpath);
                }
                catch(Exception exception)
                {
                    var diag = MessageBox.Show("Can't create a backup. Proceed anyway?","Info",MessageBoxButtons.YesNo);
                    if (diag == DialogResult.No)
                        return;
                }
            }

            var fileBytes = File.ReadAllBytes(exeFile);
            /*
            var customFunction = new byte[] { 0xA1, 0xE7, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x69, 0x00, 0x2F, 0x00, 0xA1, 0xEB, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x5B, 0x00, 0x2F, 0x00, 0xA1, 0xEF, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x4D, 0x00, 0x2F, 0x00, 0x89, 0x96, 0xD8, 0x00, 0x00, 0x00, 0xE9, 0x42, 0x00, 0x2F, 0x00 };
            var oldFunctionIndex = IndexOf(fileBytes, customFunction);
            if (oldFunctionIndex == -1)
            {
                var qolBytes = new byte[] { fileBytes[0x186129], fileBytes[0x186129+1] };
                if (qolBytes[0] == 0x74 && qolBytes[1] == 0x1B)
                {
                    //MessageBox.Show("Detected old version, going to update.");
                }
                else
                {
                    var codeCaveIndex = IndexOfCodeCave(fileBytes, 0x1FF3 - 0x1FB2);
                    if (codeCaveIndex == -1)
                    {
                        MessageBox.Show("Game is already patched or an incompatible version. Make sure this is a No-CD Mansion and Garden exe.");
                        return;
                    }


                    byte[] mainCodeLookup = new byte[] { 0x89, 0x96, 0xD8, 0x00, 0x00, 0x00, 0x8B, 0x86, 0xD8, 0x00, 0x00, 0x00, 0x3B, 0xC1, 0x0F, 0x84, 0x28, 0x02, 0x00, 0x00, 0x8B, 0x4C, 0x24, 0x0C, 0x49, 0x0F, 0x84, 0x4A };
                    var mainCodeIndex = IndexOf(fileBytes, mainCodeLookup);
                    if (mainCodeIndex == -1)
                    {
                        MessageBox.Show("Game is already patched or an incompatible version. Make sure this is a No-CD Mansion and Garden exe.");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Detected old version, going to update.");
            }
            */
            var customFunction2 = new byte[] { 0xA1, 0xE7, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x69, 0x00, 0x2F, 0x00, 0xA1, 0xEB, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x5B, 0x00, 0x2F, 0x00, 0xA1, 0xEF, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0x4D, 0x00, 0x2F, 0x00, 0xEB, 0x70, 0x90, 0x90, 0x90, 0x90, 0xE9, 0x42, 0x00, 0x2F, 0x00 };
            var jumpHook = new byte[] { 0xE9, 0x89, 0xFF, 0xD0, 0xFF, 0x90 };
            var bottomId = new byte[] { 0x01, 0x00, 0x00, 0x01 };
            var topId = new byte[] { 0x00, 0x00, 0x00, 0x01 };
            var outfitId = new byte[] { 0x02, 0x00, 0x00, 0x01 };

            //Ver2
            var buyIds = new byte[] { 0x01, 0x00, 0xC7, 0xEC, 0x00, 0x00, 0xC7, 0xEC, 0x02, 0x00, 0xC7, 0xEC };
            var newFunction = new byte[] { 0xA1, 0xF3, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0xCD, 0xFF, 0x2E, 0x00, 0xA1, 0xF7, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0xBF, 0xFF, 0x2E, 0x00, 0xA1, 0xFB, 0x1F, 0x40, 0x00, 0x39, 0x46, 0x10, 0x0F, 0x84, 0xB1, 0xFF, 0x2E, 0x00, 0x89, 0x96, 0xD8, 0x00, 0x00, 0x00, 0xE9, 0xA6, 0xFF, 0x2E, 0x00 };

            //Ver2.1
            var jumpHookQOL = new byte[] { 0xEB, 0x1B };

            ReplaceBytes(fileBytes, newFunction, 0x204E);
            ReplaceBytes(fileBytes, buyIds, 0x1FF3);

            ReplaceBytes(fileBytes, customFunction2, 0x1FB2);
            ReplaceBytes(fileBytes, jumpHook, 0x2F2024);
            ReplaceBytes(fileBytes, bottomId, 0x1FEB);
            ReplaceBytes(fileBytes, topId, 0x1FE7);
            ReplaceBytes(fileBytes, outfitId, 0x1FEF);
            ReplaceBytes(fileBytes, jumpHookQOL, 0x186129);
            try
            {
                File.WriteAllBytes(exeFile, fileBytes);
            }
            catch(Exception e)
            {
                MessageBox.Show("Couldn't patch executable. Try launching me as administrator?");
                return;
            }
            MessageBox.Show("Game Patched Successfully");
        }
    }
}

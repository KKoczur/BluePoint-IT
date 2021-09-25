using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using StatlookLogViewer.Model;

namespace StatlookLogViewer.Tools
{
    public static class ZipUtil
    {
        private static readonly string fileSeperator = "/";

        /**
        * Create a new zip file
        */
        public static ZipHandle CreateZip(string name)
        {
            ZipHandle handle = new ZipHandle();
            Stream _out = new FileStream(name, FileMode.Create);
            handle._out = new ZipOutputStream(_out);
            return handle;
        }

        /**
        * Open a zip file ( only available for read access ).
        */
        public static ZipHandle OpenZip(string name)
        {
            ZipHandle handle = new ZipHandle();
            handle._in = new ZipFile(name);
            return handle;
        }

        /**
        * Close a zip file
        */
        public static void CloseZip(ZipHandle handle)
        {
            if (handle._in != null)
                handle._in.Close();
            if (handle._out != null)
                handle._out.Close();
        }

        /**
        * Insert a file into a Zip file
        */
        public static void InsertFile(string src, string dst, ZipHandle dest)
        {
            byte[] buffer = new byte[5000];
            int read;
            if (dst == null)
                dst = src;
            else if (dst.Equals(""))
                dst = src;
            dst = ConvertToEntryName(dst);
            ZipOutputStream outputZip = dest._out;
            ZipEntry entry = new ZipEntry(dst);
            outputZip.PutNextEntry(entry);
            FileStream input = new FileStream(src, FileMode.Open);
            do
            {
                read = input.Read(buffer, 0, 5000);
                outputZip.Write(buffer, 0, read);
            }
            while (input.Position < input.Length);
            outputZip.CloseEntry();
            input.Close();
        }

        /**
        * Insert a complete directory content into a zip
        */
        public static void InsertDirectory(string path, string dst, ZipHandle dest)
        {
            string[] files;

            if (path.Equals("") || path == null)
                return;
            if (dst == null)
                dst = "";
            else
                dst = ConvertToEntryName(dst);
            if (File.Exists(path))
            {
                InsertFile(path, dst, dest);
            }
            else
            {
                files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                {

                    InsertFile(files[i],
                        (!dst.Equals("") && !dst.EndsWith(fileSeperator)) ?
                        dst + fileSeperator + ExtractFileName(files[i]) :
                        dst + ExtractFileName(files[i])
                        , dest);
                }

                files = Directory.GetDirectories(path);
                if (files == null)
                    return;
                for (int i = 0; i < files.Length; i++)
                {
                    InsertDirectory(
                            files[i],
                            (!dst.Equals("") && !dst.EndsWith(fileSeperator)) ?
                        dst + fileSeperator + ExtractFileName(files[i]) + fileSeperator :
                        dst + ExtractFileName(files[i]) + fileSeperator,
                        dest);
                }
            }
        }

        /**
        * This method just converts the '\' present in resource locations names
        * into '/' that or the only valid separator for archive entries.
        */
        public static string ConvertToEntryName(string fileName)
        {
            return fileName.Replace('\\', fileSeperator[0]);
        }

        /**
        * Return 'true' if this zip file contains the specified file
        */
        public static bool ContainsFile(ZipHandle handle, string file)
        {
            if (handle._in.GetEntry(file) != null)
                return true;
            return false;
        }

        /**
        * Return a file content from a Zip
        */
        public static byte[] GetFileContent(ZipHandle handle, string file)
        {
            ZipEntry entry = handle._in.GetEntry(file);
            byte[] content = null;
            try
            {
                Stream input = handle._in.GetInputStream(entry);
                content = new byte[(int)entry.Size];
                int index = 0;
                while (input.Position < input.Length)
                {
                    int length = input.Read(content, index, content.Length - index);
                    if (length <= 0)
                        break;
                    index += length;
                }
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("Msg : " + ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
            }
            return content;
        }

        /**
        * Return an input stream access to a zip file member
        */
        public static Stream GetInputStream(ZipHandle handle, string file)
        {
            return handle._in.GetInputStream(handle._in.GetEntry(file));
        }

        /**
        * This operation extracts a file from a ZIP archive.
        */
        public static void ExtractFile(ZipHandle handle, string output, string file)
        {
            byte[] content = GetFileContent(handle, file);
            int index = file.LastIndexOf(fileSeperator[0]);
            if (index == -1)
                index = file.LastIndexOf('\\');
            if (!output.EndsWith(System.IO.Path.DirectorySeparatorChar + ""))
                output = output + System.IO.Path.DirectorySeparatorChar;
            string output_file = output + file.Substring(index + 1, file.Length - (index + 1));
            try
            {
                FileStream fstream = new FileStream(output_file, FileMode.Create);
                fstream.Write(content, 0, content.Length);
                fstream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        /**
         * This operation returns all zip file entries that match the extension
         * provided as parameter
         */
        public static string[] GetEntriesByExtensions(ZipHandle handle, string ext)
        {
            IEnumerator _enum = handle._in.GetEnumerator();
            ArrayList list = new ArrayList();
            while (_enum.MoveNext())
            {
                string name = ((ZipEntry)_enum.Current).Name;
                if (name.EndsWith(ext))
                    list.Add(name);
            }
            string[] files = new string[list.Count];
            for (int i = 0; i < files.Length; i++)
                files[i] = (string)list[i];
            return files;
        }

        private static string ExtractFileName(string path)
        {
            path = path.Replace("\\", "/");
            int idx = path.LastIndexOf(fileSeperator);
            if (idx > -1)
            {
                return path.Substring(idx + 1);
            }
            return "";
        }

        private static string ExtractDirectoryPath(string path)
        {
            path = path.Replace("\\", "/");
            int idx = path.LastIndexOf(fileSeperator);
            if (idx > -1)
            {
                return path.Substring(0, idx);
            }
            return "";
        }
    }
}








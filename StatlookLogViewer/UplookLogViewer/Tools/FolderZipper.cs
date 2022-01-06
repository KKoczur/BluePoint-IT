using ICSharpCode.SharpZipLib.Zip;
using StatlookLogViewer.Model;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace StatlookLogViewer.Tools;

public static class ZipUtil
{
    private const string FileSeperator = "/";

    /**
        * Create a new zip file
        */
    public static ZipHandle CreateZip(string name)
    {
        var handle = new ZipHandle();
        Stream _out = new FileStream(name, FileMode.Create);
        handle.Out = new ZipOutputStream(_out);
        return handle;
    }

    /**
        * Open a zip file ( only available for read access ).
        */
    public static ZipHandle OpenZip(string name)
    {
        var handle = new ZipHandle
        {
            In = new ZipFile(name)
        };
        return handle;
    }

    /**
        * Close a zip file
        */
    public static void CloseZip(ZipHandle handle)
    {
        handle.In?.Close();
        handle.Out?.Close();
    }

    /**
        * Insert a file into a Zip file
        */
    public static void InsertFile(string src, string dst, ZipHandle dest)
    {
        var buffer = new byte[5000];
        switch (dst)
        {
            case null:
            case "":
                dst = src;
                break;
        }

        dst = ConvertToEntryName(dst);
        var outputZip = dest.Out;
        var entry = new ZipEntry(dst);
        outputZip.PutNextEntry(entry);
        var input = new FileStream(src, FileMode.Open);
        do
        {
            var read = input.Read(buffer, 0, 5000);
            outputZip.Write(buffer, 0, read);
        } while (input.Position < input.Length);

        outputZip.CloseEntry();
        input.Close();
    }

    /**
        * Insert a complete directory content into a zip
        */
    public static void InsertDirectory(string path, string dst, ZipHandle dest)
    {
        if (path.Equals(""))
            return;
        dst = dst == null ? "" : ConvertToEntryName(dst);

        if (File.Exists(path))
        {
            InsertFile(path, dst, dest);
        }
        else
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
                InsertFile(file,
                    !dst.Equals("") && !dst.EndsWith(FileSeperator)
                        ? dst + FileSeperator + ExtractFileName(file)
                        : dst + ExtractFileName(file)
                    , dest);

            files = Directory.GetDirectories(path);
            foreach (var file in files)
                InsertDirectory(
                    file,
                    !dst.Equals("") && !dst.EndsWith(FileSeperator)
                        ? dst + FileSeperator + ExtractFileName(file) + FileSeperator
                        : dst + ExtractFileName(file) + FileSeperator,
                    dest);
        }
    }

    /**
        * This method just converts the '\' present in resource locations names
        * into '/' that or the only valid separator for archive entries.
        */
    public static string ConvertToEntryName(string fileName)
    {
        return fileName.Replace('\\', FileSeperator[0]);
    }

    /**
        * Return 'true' if this zip file contains the specified file
        */
    public static bool ContainsFile(ZipHandle handle, string file)
    {
        if (handle.In.GetEntry(file) != null)
            return true;
        return false;
    }

    /**
        * Return a file content from a Zip
        */
    public static byte[] GetFileContent(ZipHandle handle, string file)
    {
        var entry = handle.In.GetEntry(file);
        byte[] content = null;
        try
        {
            var input = handle.In.GetInputStream(entry);
            content = new byte[(int)entry.Size];
            var index = 0;
            while (input.Position < input.Length)
            {
                var length = input.Read(content, index, content.Length - index);
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
        return handle.In.GetInputStream(handle.In.GetEntry(file));
    }

    /**
        * This operation extracts a file from a ZIP archive.
        */
    public static void ExtractFile(ZipHandle handle, string output, string file)
    {
        var content = GetFileContent(handle, file);
        var index = file.LastIndexOf(FileSeperator[0]);
        if (index == -1)
            index = file.LastIndexOf('\\');
        if (!output.EndsWith(Path.DirectorySeparatorChar + ""))
            output = output + Path.DirectorySeparatorChar;
        var output_file = output + file.Substring(index + 1, file.Length - (index + 1));
        try
        {
            var fstream = new FileStream(output_file, FileMode.Create);
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
        var _enum = handle.In.GetEnumerator();
        var list = new ArrayList();
        while (_enum.MoveNext())
        {
            var name = ((ZipEntry)_enum.Current).Name;
            if (name.EndsWith(ext))
                list.Add(name);
        }

        var files = new string[list.Count];
        for (var i = 0; i < files.Length; i++)
            files[i] = (string)list[i];
        return files;
    }

    private static string ExtractFileName(string path)
    {
        path = path.Replace("\\", "/");
        var idx = path.LastIndexOf(FileSeperator, StringComparison.Ordinal);
        return idx > -1 ? path.Substring(idx + 1) : "";
    }

    private static string ExtractDirectoryPath(string path)
    {
        path = path.Replace("\\", "/");
        var idx = path.LastIndexOf(FileSeperator);
        return idx > -1 ? path.Substring(0, idx) : "";
    }
}
﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
namespace Implem.Libraries.Utilities
{
    public static class Files
    {
        public enum AccessStatuses
        {
            Initialized,
            Read,
            Failed,
            NotFound
        }

        public static string Read(string path)
        {
            using (var reader = new StreamReader(path, Encoding.GetEncoding("shift_jis")))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] Bytes(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (File.Exists(path))
            {
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();
                return data;
            }
            else
            {
                return null;
            }
        }

        public static void Write(
            this string self,
            string filePath,
            string encoding = "utf-8",
            int reTryCount = 100)
        {
            var successful = false;
            var errorCount = 0;
            if (!new FileInfo(filePath).Directory.Exists)
            {
                Directory.CreateDirectory(new FileInfo(filePath).Directory.FullName);
            }
            while (!successful)
            {
                try
                {
                    using (var writer = new StreamWriter(
                        filePath,
                        false,
                        Encoding.GetEncoding(encoding)))
                    {
                        writer.Write(self);
                    }
                    successful = true;
                }
                catch
                {
                    errorCount++;
                    System.Threading.Thread.Sleep(1);
                    if (errorCount > reTryCount)
                    {
                        break;
                    }
                }
            }
        }

        public static void Write(
            this Image self,
            string filePath,
            ImageFormat format,
            int reTryCount = 100)
        {
            var successful = false;
            var errorCount = 0;
            if (!new FileInfo(filePath).Directory.Exists)
            {
                Directory.CreateDirectory(new FileInfo(filePath).Directory.FullName);
            }
            while (!successful)
            {
                try
                {
                    self.Save(filePath, format);
                    successful = true;
                }
                catch
                {
                    errorCount++;
                    System.Threading.Thread.Sleep(1);
                    if (errorCount > reTryCount)
                    {
                        break;
                    }
                }
            }
        }

        public static bool FileExists(this string self) 
        { 
            return File.Exists(self); 
        }

        public static string FileNameOnly(this string self) 
        {
            return Path.ChangeExtension(Path.GetFileName(self), null);
        }

        public static string FileExtension(this string self) 
        {
            return new FileInfo(self).Extension;
        }

        public static string DirectoryName(this string self) 
        {
            return new FileInfo(self).DirectoryName;
        }

        public static string ValidFileName(string fileName)
        {
            var validFileName = fileName;
            "\\/：*?\"<>|".ForEach(invalidChar =>
            {
                if (validFileName.Contains(invalidChar))
                {
                    validFileName = validFileName.Replace(invalidChar, '_');
                }
            });
            return validFileName;
        }

        public static void DeleteDirectory(
            string path, string searchPattern = "", bool recursive = true)
        {
            new DirectoryInfo(path: path)
                .GetDirectories(searchPattern: searchPattern)
                .ForEach(o => o.Delete(recursive: true));
        }

        public static void CopyDirectory(
            string sourcePath,
            string destinationPath, 
            IEnumerable<string> excludePathCollection, 
            FileAttributes excludeFileAttributes = FileAttributes.Hidden,
            FileAttributes excludeDirectoryAttributes = FileAttributes.Hidden,
            bool overwrite = true)
        {
            if (excludePathCollection.Any(o => destinationPath.ToLower().EndsWith(o.ToLower())))
            {
                return;
            }
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
                File.SetAttributes(destinationPath, File.GetAttributes(sourcePath));
            }
            Directory.EnumerateFiles(sourcePath)
                .Where(o => (File.GetAttributes(o) & excludeFileAttributes) !=
                    excludeFileAttributes)
                .ForEach(sourceFilePath => File.Copy(sourceFilePath, Path.Combine(
                    destinationPath, Path.GetFileName(sourceFilePath)), overwrite));
            Directory.EnumerateDirectories(sourcePath)
                .Where(o => (new DirectoryInfo(o).Attributes & excludeDirectoryAttributes) !=
                    excludeDirectoryAttributes)
                .ForEach(subDirectoryPath => CopyDirectory(
                    subDirectoryPath,
                    Path.Combine(destinationPath, Path.GetFileName(subDirectoryPath)),
                    excludePathCollection));
        }

        public static string CopyToTemp(string sourcePath, string tempPath)
        {
            var sourceFile = new FileInfo(sourcePath);
            var path = Path.Combine(tempPath, Strings.NewGuid() + sourceFile.Extension);
            sourceFile.CopyTo(path);
            return path;
        }

        public static FileInfo WriteToTemp(
            this string self, string path, string encoding = "utf-8")
        {
            var tempPath = Path.Combine(path, Strings.NewGuid() + ".txt");
            self.Write(tempPath, encoding);
            return new FileInfo(tempPath);
        }

        public static void DeleteHistoryFile(string directoryPath, int timeElapsed)
        {
            Directory.EnumerateFiles(directoryPath).ForEach(filePath =>
            {
                var fileInfo = new FileInfo(filePath);
                if ((DateTime.Now - fileInfo.LastWriteTime).TotalMinutes > timeElapsed)
                {
                    fileInfo.Delete();
                }
            });
        }
    }
}

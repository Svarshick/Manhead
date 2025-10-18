#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveableAttribute : Attribute
    {
        public readonly string FileName;
        public SaveableAttribute(string FileName)
        {
            this.FileName = FileName;
        }
    }
    public class SaveFileHandle : IDisposable
    {
        public class SaveHandleInitializationSystemException : Exception
        {
            /// <summary>
            /// Indicates if u can run futher ur application after exception occured.
            /// </summary>
            public readonly bool Restorable;
            public SaveHandleInitializationSystemException(string msg, Exception innerException, bool rest = false) : base(msg, innerException)
            {
                Restorable = rest;
            }
        }

        public class SaveHandleInitializationUserException : Exception
        {
            public SaveHandleInitializationUserException(string msg, Exception innerException) : base(msg, innerException) { }
        }

        public FileStream InternalStream { get; internal set; }
        private SaveFileHandle(FileStream stream)
        {
            InternalStream = stream;
        }

        /// <summary>
        /// Creates safe handle and locks file owning.
        /// </summary>
        /// <param name="file">Path to target file.</param>
        /// <param name="createFile">If we should create file or just open.</param>
        /// <param name="createDirs">If we should create directories if they not exsists.</param>
        /// <returns></returns>
        /// <exception cref="SaveHandleInitializationUserException">For exceptions that may be handled by code.</exception>
        /// <exception cref="SaveHandleInitializationSystemException">For exceptions that can not be handled by code.</exception>
        public static SaveFileHandle CreateHandle(string file, bool createFile = false, bool createDirs = false)
        {
            try
            {
                if (createDirs)
                {
                    string? directoryPath = Path.GetDirectoryName(file);
                    if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }
                var fs = new FileStream(file, createFile ? FileMode.OpenOrCreate : FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return new SaveFileHandle(fs);
            }
            catch (ArgumentNullException ex)
            {
                throw new SaveHandleInitializationUserException("Passed null as file path argument.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                var mode = createFile ? "Create|Open" : "Open";
                throw new SaveHandleInitializationSystemException($"Fore some reason we cann't {mode} file.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new SaveHandleInitializationUserException("Passed incorrect file path argument.", ex);
            }
            catch (NotSupportedException ex)
            {
                throw new SaveHandleInitializationSystemException("File not supported on the FileSystem.", ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new SaveHandleInitializationUserException("Passed unexisted file path argument. Use createFile = true for auto handling", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new SaveHandleInitializationUserException("Passed file path from unexisted directory. Use createDirs = true for auto handling", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new SaveHandleInitializationSystemException($"Program have no rights to access {file}.", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new SaveHandleInitializationSystemException($"Passed argument: \"{file}\" too long dor this system.", ex);
            }
            catch (IOException ex)
            {
                throw new SaveHandleInitializationSystemException("Fail due system error.", ex);
            }
        }

        public void Dispose()
        {
            InternalStream.Flush();
            InternalStream.Dispose();
        }
    }
    public class FileLock : IDisposable
    {
        public SaveFileHandle Handle { get; internal set; }
        public FileStream Stream => Handle.InternalStream;
        public FileInfo Info { get; internal set; }
        private FileLock(SaveFileHandle Path, FileInfo fifo)
        {
            Handle = Path;
            Info = fifo;
        }

        public StreamReader KeepAliveReader()
        {
            return new StreamReader(Handle.InternalStream, Encoding.UTF8, true, 1024, true);
        }

        public StreamWriter KeepAliveWriter()
        {
            return new StreamWriter(Handle.InternalStream, Encoding.UTF8, 1024, leaveOpen: true);
        }

        public static FileLock? Lock(string path, bool createFile = false, bool createDirs = false)
        {
            try
            {
                var handle = SaveFileHandle.CreateHandle(path, createFile, createDirs);
                return new FileLock(handle, new FileInfo(path));
            }
            catch (SaveFileHandle.SaveHandleInitializationUserException ex)
            {
                return null;
            }
        }

        public void Dispose()
        {
            Handle.Dispose();
        }
    }
    public class DirectoryLock : IDisposable
    {
        public FileLock Handle { get; internal set; }
        public DirectoryInfo Directory { get; internal set; }
        private DirectoryLock(FileLock Path, DirectoryInfo difo)
        {
            Handle = Path;
            Directory = difo;
        }

        public static DirectoryLock? Lock(string path, bool createDirs = false)
        {
            var lock_path = Path.Combine(path, "Lock.lock");
            var handle = FileLock.Lock(lock_path, true, createDirs);
            if (handle != null)
                return new DirectoryLock(handle, new DirectoryInfo(path));
            else
                return null;
        }

        public void Dispose()
        {
            var file = Handle.Info;
            Handle.Dispose();
            file.Delete();
        }
    }
    public class Save : IDisposable
    {
        public FileLock File { get; internal set; }

        internal Save(FileLock File)
        {
            this.File = File;
        }

        public string ReadAllText()
        {
            File.Stream.Seek(0, SeekOrigin.Begin);
            return File.KeepAliveReader().ReadToEnd();
        }

        public void Append(string text)
        {
            File.KeepAliveWriter().Write(text);
        }

        public void ClearAndWrite(string text)
        {
            File.Stream.Seek(0, SeekOrigin.Begin);
            File.KeepAliveWriter().Write(text);
            File.Stream.SetLength(File.Stream.Position);
        }

        public static Save? Open(string path, bool createFile = false)
        {
            var file = FileLock.Lock(path, createFile);
            if (file != null)
                return new Save(file);
            return null;
        }

        public void Dispose()
        {
            File.Dispose();
        }
    }
    public class GameSave : IDisposable
    {
        public DirectoryLock Directory { get; internal set; }

        internal GameSave(DirectoryLock dir)
        {
            Directory = dir;
        }

        public Save? OpenSave<T>()
        {
            var attr = typeof(T).GetCustomAttribute<SaveableAttribute>();
            if (attr == null)
                return null;
            return OpenOrCreateByName(attr.FileName, false).Save;
        }

        public (Save? Save, bool IsCreated) OpenOrCreateByName(string name, bool create = true)
        {
            var path = Path.Combine(this.Directory.Directory.FullName, name);
            var s = Save.Open(path);
            if (s != null)
                return (s, false);
            if (!create)
                return (null, false);
            return (Save.Open(path, true), true);
        }

        public (Save? Save, bool IsCreated) OpenOrCreateSave<T>()
        {
            var attr = typeof(T).GetCustomAttribute<SaveableAttribute>();
            if (attr == null)
                return (null, false);
            return OpenOrCreateByName(attr.FileName);
        }

        public static GameSave? Open(string path, bool create = false)
        {
            var dir = DirectoryLock.Lock(path, create);
            if (dir != null)
                return new GameSave(dir);
            return null;
        }

        public void Dispose()
        {
            Directory.Dispose();
        }
    }
    public class SaveStorage : IDisposable
    {
        public GameSave General { get; }

        public SaveStorage(string Path)
        {
            General = new GameSave(DirectoryLock.Lock(Path, true) ?? throw new ArgumentException("Failed to Open&Lock target save storage."));
        }

        public List<string> ListNames()
        {
            List<string> saves = new List<string>();
            foreach(var save in General.Directory.Directory.EnumerateDirectories())
            {
                using var s = GameSave.Open(save.FullName);
                if (s != null)
                    saves.Add(s.Directory.Directory.Name);
            }
            return saves;
        }

        public GameSave? OpenOrCreateGameSave(string name, bool create = true)
        {
            var path = Path.Combine(General.Directory.Directory.FullName, name);
            return GameSave.Open(path, create);
        }

        public GameSave? CreateSave(string name)
        {
            var path = Path.Combine(General.Directory.Directory.FullName, name);
            var dlock = DirectoryLock.Lock(path, true);
            if (dlock == null)
                return null;
            return new GameSave(dlock);
        }

        public void Dispose()
        {
            General.Dispose();
        }
    }
}
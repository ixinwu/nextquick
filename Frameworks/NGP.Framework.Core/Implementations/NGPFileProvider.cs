/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * NGPFileProvider Description:
 * NGP�ļ��ṩ��(IO����ʹ�ô����ϵ��ļ�ϵͳ)
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-28   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace NGP.Framework.Core
{
    /// <summary>
    /// NGP�ļ��ṩ��(IO����ʹ�ô����ϵ��ļ�ϵͳ)
    /// </summary>
    public class NGPFileProvider : PhysicalFileProvider, INGPFileProvider
    {
        /// <summary>
        /// ��ʼ��NGPFileProvider����ʵ��
        /// </summary>
        /// <param name="hostingEnvironment">�йܻ���</param>
        public NGPFileProvider(string path = null)
            : base(string.IsNullOrWhiteSpace(path) ? AppContext.BaseDirectory : path)
        {
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            BaseDirectory = path;
        }

        #region Utilities

        private static void DeleteDirectoryRecursive(string path)
        {
            Directory.Delete(path, true);
            const int maxIterationToWait = 10;
            var curIteration = 0;

            //according to the documentation(https://msdn.microsoft.com/ru-ru/library/windows/desktop/aa365488.aspx) 
            //System.IO.Directory.Delete method ultimately (after removing the files) calls native 
            //RemoveDirectory function which marks the directory as "deleted". That's why we wait until 
            //the directory is actually deleted. For more details see https://stackoverflow.com/a/4245121
            while (Directory.Exists(path))
            {
                curIteration += 1;
                if (curIteration > maxIterationToWait)
                    return;
                Thread.Sleep(100);
            }
        }

        #endregion

        /// <summary>
        /// ���ַ���������ϵ�·����
        /// </summary>
        /// <param name="paths">·�����ֵ�����</param>
        /// <returns>���·��</returns>
        public virtual string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// ��ָ��·���д�������Ŀ¼����Ŀ¼�����������Ѿ����ڡ�
        /// </summary>
        /// <param name="path">Ҫ������Ŀ¼</param>
        public virtual void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// ��ָ��·���д����򸲸��ļ�
        /// </summary>
        /// <param name="path">Ҫ�������ļ���·��������</param>
        public virtual void CreateFile(string path)
        {
            if (FileExists(path))
                return;

            //we use 'using' to close the file after it's created
            using (File.Create(path))
            {
            }
        }

        /// <summary>
        /// ������ȵݹ�ɾ����������Windows��Դ�������д򿪵���Ŀ¼��
        /// </summary>
        /// <param name="path">Ŀ¼·��</param>
        public void DeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);

            //find more info about directory deletion
            //and why we use this approach at https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true

            foreach (var directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                DeleteDirectoryRecursive(path);
            }
            catch (IOException)
            {
                DeleteDirectoryRecursive(path);
            }
            catch (UnauthorizedAccessException)
            {
                DeleteDirectoryRecursive(path);
            }
        }

        /// <summary>
        /// ɾ��ָ�����ļ�
        /// </summary>
        /// <param name="filePath">Ҫɾ�����ļ������ơ���֧��ͨ���</param>
        public virtual void DeleteFile(string filePath)
        {
            if (!FileExists(filePath))
                return;

            File.Delete(filePath);
        }

        /// <summary>
        /// ȷ������·���Ƿ����ô����ϵ�����Ŀ¼
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// ���·����������Ŀ¼����Ϊtrue�����Ŀ¼�����ڻ��ڳ���ȷ��ָ���ļ��Ƿ����ʱ����������Ϊfalse
        /// </returns>
        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// ���ļ���Ŀ¼���������ƶ�����λ��
        /// </summary>
        /// <param name="sourceDirName">Ҫ�ƶ����ļ���Ŀ¼��·��</param>
        /// <param name="destDirName">
        /// sourcedirname����λ�õ�·�������sourcedirname��һ���ļ�����ôdestdirnameҲ������һ���ļ���
        /// </param>
        public virtual void DirectoryMove(string sourceDirName, string destDirName)
        {
            Directory.Move(sourceDirName, destDirName);
        }

        /// <summary>
        /// ������ָ��·���е�����ģʽƥ����ļ����Ŀ�ö�ټ��ϣ�������ѡ��������Ŀ¼��
        /// </summary>
        /// <param name="directoryPath">Ҫ������Ŀ¼��·��</param>
        /// <param name="searchPattern">
        /// ��·�����ļ���ƥ��������ַ���������������԰�����Ч���ı�·����ͨ�����*and��������ϡ����֣�����֧��������ʽ��
        /// </param>
        /// <param name="topDirectoryOnly">
        /// ָ����������ǰĿ¼������������ǰĿ¼��������Ŀ¼
        /// </param>
        /// <returns>
        /// ��pathָ����Ŀ¼�е��ļ���ȫ��������·�����Ŀ�ö�ټ��ϣ���Щ�ļ���ָ��������ģʽƥ�䡣
        /// </returns>
        public virtual IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern,
            bool topDirectoryOnly = true)
        {
            return Directory.EnumerateFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        /// <summary>
        /// �������ļ����Ƶ����ļ���������ͬ���ļ�
        /// </summary>
        /// <param name="sourceFileName">Ҫ���Ƶ��ļ�</param>
        /// <param name="destFileName">Ŀ���ļ������ơ��ⲻ����Ŀ¼</param>
        /// <param name="overwrite">������Ը���Ŀ���ļ�����Ϊtrue������Ϊfalse</param>
        public virtual void FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        /// <summary>
        /// ȷ��ָ�����ļ��Ƿ����
        /// </summary>
        /// <param name="filePath">Ҫ�����ļ�</param>
        /// <returns>
        /// ������÷����������Ȩ�ޣ�����·�����������ļ������ƣ���Ϊtrue������Ϊfalse��
        /// </returns>
        public virtual bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// ��ȡ�ļ��ĳ��ȣ����ֽ�Ϊ��λ��������Ŀ¼�򲻴��ڵ��ļ�����Ϊ-1
        /// </summary>
        /// <param name="path">�ļ�·��</param>
        /// <returns>�ļ��ĳ���</returns>
        public virtual long FileLength(string path)
        {
            if (!FileExists(path))
                return -1;

            return new FileInfo(path).Length;
        }

        /// <summary>
        /// ��ָ�����ļ��ƶ�����λ�ã��ṩָ�����ļ�����ѡ��
        /// </summary>
        /// <param name="sourceFileName">Ҫ�ƶ����ļ������ơ����԰������·�������·��</param>
        /// <param name="destFileName">�ļ�����·��������</param>
        public virtual void FileMove(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        /// <summary>
        /// ����Ŀ¼�ľ���·��
        /// </summary>
        /// <param name="paths">·�����ֵ�����</param>
        /// <returns>Ŀ¼�ľ���·��</returns>
        public virtual string GetAbsolutePath(params string[] paths)
        {
            var allPaths = paths.ToList();
            allPaths.Insert(0, Root);

            return Path.Combine(allPaths.ToArray());
        }

        /// <summary>
        /// ��ȡ���ڷ�װָ��Ŀ¼�ķ��ʿ����б�ACL�����System.Security.AccessControl.DirectorySecurity����
        /// </summary>
        /// <param name="path">���������ļ����ʿ����б�ACL����Ϣ��System.Security.AccessControl.DirectorySecurity�����Ŀ¼·��</param>
        /// <returns>��װ��path�����������ļ��ķ��ʿ��ƹ���Ķ���</returns>
        public virtual DirectorySecurity GetAccessControl(string path)
        {
            return new DirectoryInfo(path).GetAccessControl();
        }

        /// <summary>
        /// ����ָ���ļ���Ŀ¼�Ĵ������ں�ʱ��
        /// </summary>
        /// <param name="path">��ȡ�������ں�ʱ����Ϣ���ļ���Ŀ¼��</param>
        /// <returns>
        /// ����Ϊָ���ļ���Ŀ¼�Ĵ������ں�ʱ���System.DateTime�ṹ����ֵ�Ե���ʱ���ʾ
        /// </returns>
        public virtual DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        /// <summary>
        /// ������ָ��Ŀ¼��ָ������ģʽƥ�����Ŀ¼��������·����������
        /// </summary>
        /// <param name="path">Ҫ������Ŀ¼��·��</param>
        /// <param name="searchPattern">
        /// ��·������Ŀ¼������ƥ��������ַ������˲������԰�����Ч�����ֺ�ͨ�������ϣ�����֧��������ʽ��
        /// </param>
        /// <param name="topDirectoryOnly">
        /// ָ����������ǰĿ¼������������ǰĿ¼��������Ŀ¼
        /// </param>
        /// <returns>
        /// ��ָ������ƥ�����Ŀ¼��ȫ��������·�������飻����Ҳ���Ŀ¼����Ϊ�����顣
        /// </returns>
        public virtual string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "*";

            return Directory.GetDirectories(path, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        /// <summary>
        /// ����ָ��·���ַ�����Ŀ¼��Ϣ
        /// </summary>
        /// <param name="path">�ļ���Ŀ¼��·��</param>
        /// <returns>
        /// ·����Ŀ¼��Ϣ�����·����ʾ��Ŀ¼��Ϊ�գ���Ϊ�ա����·��������Ŀ¼��Ϣ���򷵻�System.String.Empty
        /// </returns>
        public virtual string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// ������ָ��·���ַ�����Ŀ¼��
        /// </summary>
        /// <param name="path">Ŀ¼·��</param>
        /// <returns>Ŀ¼��</returns>
        public virtual string GetDirectoryNameOnly(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        /// <summary>
        /// ����ָ��·���ַ�������չ��
        /// </summary>
        /// <param name="filePath">���л�ȡ��չ����·���ַ���</param>
        /// <returns>ָ��·������չ����������㡰.������</returns>
        public virtual string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// ����ָ��·���ַ������ļ�������չ��
        /// </summary>
        /// <param name="path">���л�ȡ�ļ�������չ����·���ַ���</param>
        /// <returns>·�������һ��Ŀ¼�ַ�����ַ�</returns>
        public virtual string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// ���ز�����չ����ָ��·���ַ������ļ���
        /// </summary>
        /// <param name="filePath">�ļ���·��</param>
        /// <returns>�ļ�������ȥ���һ����㣨.���������������ַ�</returns>
        public virtual string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// ������ָ��Ŀ¼��ָ������ģʽƥ����ļ�����������·������ʹ��ֵȷ���Ƿ�������Ŀ¼��
        /// </summary>
        /// <param name="directoryPath">Ҫ������Ŀ¼��·��</param>
        /// <param name="searchPattern">
        /// ��·�����ļ���ƥ��������ַ������˲������԰�����Ч���ı�·����ͨ�����*and��������ϡ��ַ�������֧��������ʽ��
        /// </param>
        /// <param name="topDirectoryOnly">
        /// ָ����������ǰĿ¼������������ǰĿ¼��������Ŀ¼
        /// </param>
        /// <returns>
        /// ָ��Ŀ¼����ָ������ģʽƥ����ļ���ȫ��������·�������飻����Ҳ����ļ�����Ϊ�����顣
        /// </returns>
        public virtual string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "*.*";

            return Directory.GetFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        /// <summary>
        /// �����ϴη���ָ���ļ���Ŀ¼�����ں�ʱ��
        /// </summary>
        /// <param name="path">Ҫ��ȡ�������ں�ʱ����Ϣ���ļ���Ŀ¼</param>
        /// <returns>System.DateTime�ṹ����Ϊָ���ļ������ں�ʱ��</returns>
        public virtual DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        /// <summary>
        /// �����ϴ�д��ָ���ļ���Ŀ¼�����ں�ʱ��
        /// </summary>
        /// <param name="path">��ȡд�����ں�ʱ����Ϣ���ļ���Ŀ¼��</param>
        /// <returns>
        /// ����Ϊָ���ļ���Ŀ¼�ϴ�д������ں�ʱ���System.DateTime�ṹ����ֵ�Ա���ʱ���ʾ
        /// </returns>
        public virtual DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        /// <summary>
        /// ��Э������ʱ��UTC������ָ���ļ���Ŀ¼�ϴ�д������ں�ʱ�䡣
        /// </summary>
        /// <param name="path">��ȡд�����ں�ʱ����Ϣ���ļ���Ŀ¼��</param>
        /// <returns>
        /// ����Ϊָ���ļ���Ŀ¼�ϴ�д������ں�ʱ���System.DateTime�ṹ����ֵ��UTCʱ���ʾ��
        /// </returns>
        public virtual DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        /// <summary>
        /// ����ָ��·���ĸ�Ŀ¼
        /// </summary>
        /// <param name="directoryPath">������Ŀ¼��·��</param>
        /// <returns>��Ŀ¼�����path�Ǹ�Ŀ¼����Ϊ�գ�����UNC�������ĸ�Ŀ¼��������</returns>
        public virtual string GetParentDirectory(string directoryPath)
        {
            return Directory.GetParent(directoryPath).FullName;
        }

        /// <summary>
        /// ���·���Ƿ�ΪĿ¼
        /// </summary>
        /// <param name="path">���·��</param>
        /// <returns>���·����Ŀ¼����Ϊtrue������Ϊfalse</returns>
        public virtual bool IsDirectory(string path)
        {
            return DirectoryExists(path);
        }

        /// <summary>
        /// ������·��ӳ�䵽�������·��
        /// </summary>
        /// <param name="path">��~/bin��</param>
        /// <returns>����·�� path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            path = path.Replace("~/", string.Empty).TrimStart('/').Replace('/', '\\');
            return Path.Combine(BaseDirectory ?? string.Empty, path);
        }

        /// <summary>
        /// ���ļ����ݶ�ȡ���ֽ�������
        /// </summary>
        /// <param name="filePath">���ڶ�ȡ���ļ�</param>
        /// <returns>�����ļ����ݵ��ֽ�����</returns>
        public virtual byte[] ReadAllBytes(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : new byte[0];
        }

        /// <summary>
        /// ���ļ�����ָ���ı����ȡ�ļ��������У�Ȼ��ر��ļ���
        /// </summary>
        /// <param name="path">Ҫ���Թ���ȡ���ļ�</param>
        /// <param name="encoding">Ӧ�����ļ����ݵı���</param>
        /// <returns>�����ļ������е��ַ���</returns>
        public virtual string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        /// <summary>
        /// ��Э������ʱ��UTC��Ϊ��λ�����ϴ�д��ָ���ļ������ں�ʱ�䡣
        /// </summary>
        /// <param name="path">�������ں�ʱ����Ϣ���ļ�</param>
        /// <param name="lastWriteTimeUtc">
        /// ����ҪΪ·�������һ��д�����ں�ʱ�����õ�ֵ��System.DateTime����ֵ��UTCʱ���ʾ
        /// </param>
        public virtual void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        /// <summary>
        /// ��ָ�����ֽ�����д���ļ�
        /// </summary>
        /// <param name="filePath">д����ļ�</param>
        /// <param name="bytes">�ļ��ֽ�����</param>
        public virtual void WriteAllBytes(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }

        /// <summary>
        /// �������ļ���ʹ��ָ���ı��뽫ָ�����ַ���д���ļ���Ȼ��رո��ļ������Ŀ���ļ��Ѿ����ڣ����������ǡ�
        /// </summary>
        /// <param name="path">Ҫд����ļ�</param>
        /// <param name="contents">Ҫд���ļ����ַ���</param>
        /// <param name="encoding">ҪӦ�����ַ����ı���</param>
        public virtual void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        /// <summary>
        /// ��ȡ�ļ�����
        /// </summary>
        /// <typeparam name="T">�����л�����</typeparam>
        /// <param name="filePath">�ļ�·��</param>
        /// <returns>�����л�����</returns>
        public T GetFileContent<T>(string filePath) where T : new()
        {
            var text = ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new T();

            // ��JSON�ļ��л�ȡ���ϵͳ����
            return JsonConvert.DeserializeObject<T>(text);
        }

        /// <summary>
        /// ��·��
        /// </summary>
        protected string BaseDirectory { get; }
    }
}
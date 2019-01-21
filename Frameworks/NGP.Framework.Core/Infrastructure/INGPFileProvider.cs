﻿/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * INGPFileProvider Description:
 * ngp文件提供者
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace NGP.Framework.Core
{
    /// <summary>
    /// ngp文件提供者
    /// </summary>
    public interface INGPFileProvider : IFileProvider
    {
        /// <summary>
        /// 将字符串数组组合到路径中
        /// </summary>
        /// <param name="paths">路径部分的数组</param>
        /// <returns>组合路径</returns>
        string Combine(params string[] paths);

        /// <summary>
        /// 在指定路径中创建所有目录和子目录，除非它们已经存在。
        /// </summary>
        /// <param name="path">要创建的目录</param>
        void CreateDirectory(string path);

        /// <summary>
        /// 在指定路径中创建或覆盖文件
        /// </summary>
        /// <param name="path">要创建的文件的路径和名称</param>
        void CreateFile(string path);

        /// <summary>
        /// 深度优先递归删除，处理在Windows资源管理器中打开的子目录。
        /// </summary>
        /// <param name="path">目录路径</param>
        void DeleteDirectory(string path);

        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="filePath">要删除的文件的名称。不支持通配符</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// 确定给定路径是否引用磁盘上的现有目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// 如果路径引用现有目录，则为true；如果目录不存在或在尝试确定指定文件是否存在时发生错误，则为false
        /// </returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// 将文件或目录及其内容移动到新位置
        /// </summary>
        /// <param name="sourceDirName">要移动的文件或目录的路径</param>
        /// <param name="destDirName">
        /// sourcedirname的新位置的路径。如果sourcedirname是一个文件，那么destdirname也必须是一个文件名
        /// </param>
        void DirectoryMove(string sourceDirName, string destDirName);

        /// <summary>
        /// 返回与指定路径中的搜索模式匹配的文件名的可枚举集合，并可以选择搜索子目录。
        /// </summary>
        /// <param name="directoryPath">要搜索的目录的路径</param>
        /// <param name="searchPattern">
        /// 与路径中文件名匹配的搜索字符串。这个参数可以包含有效的文本路径和通配符（*and？）的组合。文字，但不支持正则表达式。
        /// </param>
        /// <param name="topDirectoryOnly">
        /// 指定是搜索当前目录，还是搜索当前目录和所有子目录
        /// </param>
        /// <returns>
        /// 由path指定的目录中的文件的全名（包括路径）的可枚举集合，这些文件与指定的搜索模式匹配。
        /// </returns>
        IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern, bool topDirectoryOnly = true);

        /// <summary>
        /// 将现有文件复制到新文件。允许覆盖同名文件
        /// </summary>
        /// <param name="sourceFileName">要复制的文件</param>
        /// <param name="destFileName">目标文件的名称。这不能是目录</param>
        /// <param name="overwrite">如果可以覆盖目标文件，则为true；否则为false</param>
        void FileCopy(string sourceFileName, string destFileName, bool overwrite = false);

        /// <summary>
        /// 确定指定的文件是否存在
        /// </summary>
        /// <param name="filePath">要检查的文件</param>
        /// <returns>
        /// 如果调用方具有所需的权限，并且路径包含现有文件的名称，则为true；否则为false。
        /// </returns>
        bool FileExists(string filePath);

        /// <summary>
        /// 获取文件的长度（以字节为单位），对于目录或不存在的文件，则为-1
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件的长度</returns>
        long FileLength(string path);

        /// <summary>
        /// 将指定的文件移动到新位置，提供指定新文件名的选项
        /// </summary>
        /// <param name="sourceFileName">要移动的文件的名称。可以包括相对路径或绝对路径</param>
        /// <param name="destFileName">文件的新路径和名称</param>
        void FileMove(string sourceFileName, string destFileName);

        /// <summary>
        /// 返回目录的绝对路径
        /// </summary>
        /// <param name="paths">路径部分的数组</param>
        /// <returns>目录的绝对路径</returns>
        string GetAbsolutePath(params string[] paths);

        /// <summary>
        /// 获取用于封装指定目录的访问控制列表（ACL）项的System.Security.AccessControl.DirectorySecurity对象
        /// </summary>
        /// <param name="path">包含描述文件访问控制列表（ACL）信息的System.Security.AccessControl.DirectorySecurity对象的目录路径</param>
        /// <returns>封装由path参数描述的文件的访问控制规则的对象。</returns>
        DirectorySecurity GetAccessControl(string path);

        /// <summary>
        /// 返回指定文件或目录的创建日期和时间
        /// </summary>
        /// <param name="path">获取创建日期和时间信息的文件或目录。</param>
        /// <returns>
        /// 设置为指定文件或目录的创建日期和时间的System.DateTime结构。该值以当地时间表示
        /// </returns>
        DateTime GetCreationTime(string path);

        /// <summary>
        /// 返回与指定目录中指定搜索模式匹配的子目录（包括其路径）的名称
        /// </summary>
        /// <param name="path">要搜索的目录的路径</param>
        /// <param name="searchPattern">
        /// 与路径中子目录的名称匹配的搜索字符串。此参数可以包含有效的文字和通配符的组合，但不支持正则表达式。
        /// </param>
        /// <param name="topDirectoryOnly">
        /// 指定是搜索当前目录，还是搜索当前目录和所有子目录
        /// </param>
        /// <returns>
        /// 与指定条件匹配的子目录的全名（包括路径）数组；如果找不到目录，则为空数组。
        /// </returns>
        string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true);

        /// <summary>
        /// 返回指定路径字符串的目录信息
        /// </summary>
        /// <param name="path">文件或目录的路径</param>
        /// <returns>
        /// 路径的目录信息，如果路径表示根目录或为空，则为空。如果路径不包含目录信息，则返回System.String.Empty
        /// </returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// 仅返回指定路径字符串的目录名
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>目录名</returns>
        string GetDirectoryNameOnly(string path);

        /// <summary>
        /// 返回指定路径字符串的扩展名
        /// </summary>
        /// <param name="filePath">从中获取扩展名的路径字符串</param>
        /// <returns>指定路径的扩展名（包括句点“.”）。</returns>
        string GetFileExtension(string filePath);

        /// <summary>
        /// 返回指定路径字符串的文件名和扩展名
        /// </summary>
        /// <param name="path">从中获取文件名和扩展名的路径字符串</param>
        /// <returns>路径中最后一个目录字符后的字符</returns>
        string GetFileName(string path);

        /// <summary>
        /// 返回不带扩展名的指定路径字符串的文件名
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        /// <returns>文件名，减去最后一个句点（.）及其后面的所有字符</returns>
        string GetFileNameWithoutExtension(string filePath);

        /// <summary>
        /// 返回与指定目录中指定搜索模式匹配的文件名（包括其路径），使用值确定是否搜索子目录。
        /// </summary>
        /// <param name="directoryPath">要搜索的目录的路径</param>
        /// <param name="searchPattern">
        /// 与路径中文件名匹配的搜索字符串。此参数可以包含有效的文本路径和通配符（*and？）的组合。字符，但不支持正则表达式。
        /// </param>
        /// <param name="topDirectoryOnly">
        /// 指定是搜索当前目录，还是搜索当前目录和所有子目录
        /// </param>
        /// <returns>
        /// 指定目录中与指定搜索模式匹配的文件的全名（包括路径）数组；如果找不到文件，则为空数组。
        /// </returns>
        string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true);

        /// <summary>
        /// 返回上次访问指定文件或目录的日期和时间
        /// </summary>
        /// <param name="path">要获取访问日期和时间信息的文件或目录</param>
        /// <returns>System.DateTime结构设置为指定文件的日期和时间</returns>
        DateTime GetLastAccessTime(string path);

        /// <summary>
        /// 返回上次写入指定文件或目录的日期和时间
        /// </summary>
        /// <param name="path">获取写入日期和时间信息的文件或目录。</param>
        /// <returns>
        /// 设置为指定文件或目录上次写入的日期和时间的System.DateTime结构。此值以本地时间表示
        /// </returns>
        DateTime GetLastWriteTime(string path);

        /// <summary>
        /// 以协调世界时（UTC）返回指定文件或目录上次写入的日期和时间。
        /// </summary>
        /// <param name="path">获取写入日期和时间信息的文件或目录。</param>
        /// <returns>
        /// 设置为指定文件或目录上次写入的日期和时间的System.DateTime结构。此值以UTC时间表示。
        /// </returns>
        DateTime GetLastWriteTimeUtc(string path);

        /// <summary>
        /// 检索指定路径的父目录
        /// </summary>
        /// <param name="directoryPath">检索父目录的路径</param>
        /// <returns>父目录，如果path是根目录，则为空，包括UNC服务器的根目录或共享名。</returns>
        string GetParentDirectory(string directoryPath);

        /// <summary>
        /// 检查路径是否为目录
        /// </summary>
        /// <param name="path">检查路径</param>
        /// <returns>如果路径是目录，则为true，否则为false</returns>
        bool IsDirectory(string path);

        /// <summary>
        /// 将虚拟路径映射到物理磁盘路径
        /// </summary>
        /// <param name="path">“~/bin”</param>
        /// <returns>物理路径 path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>
        /// 将文件内容读取到字节数组中
        /// </summary>
        /// <param name="filePath">用于读取的文件</param>
        /// <returns>包含文件内容的字节数组</returns>
        byte[] ReadAllBytes(string filePath);

        /// <summary>
        /// 打开文件，用指定的编码读取文件的所有行，然后关闭文件。
        /// </summary>
        /// <param name="path">要打开以供读取的文件</param>
        /// <param name="encoding">应用于文件内容的编码</param>
        /// <returns>包含文件所有行的字符串</returns>
        string ReadAllText(string path, Encoding encoding);

        /// <summary>
        /// 以协调世界时（UTC）为单位设置上次写入指定文件的日期和时间。
        /// </summary>
        /// <param name="path">设置日期和时间信息的文件</param>
        /// <param name="lastWriteTimeUtc">
        /// 包含要为路径的最后一个写入日期和时间设置的值的System.DateTime。此值以UTC时间表示
        /// </param>
        void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

        /// <summary>
        /// 将指定的字节数组写入文件
        /// </summary>
        /// <param name="filePath">写入的文件</param>
        /// <param name="bytes">文件字节数组</param>
        void WriteAllBytes(string filePath, byte[] bytes);

        /// <summary>
        /// 创建新文件，使用指定的编码将指定的字符串写入文件，然后关闭该文件。如果目标文件已经存在，它将被覆盖。
        /// </summary>
        /// <param name="path">要写入的文件</param>
        /// <param name="contents">要写入文件的字符串</param>
        /// <param name="encoding">要应用于字符串的编码</param>
        void WriteAllText(string path, string contents, Encoding encoding);
    }
}
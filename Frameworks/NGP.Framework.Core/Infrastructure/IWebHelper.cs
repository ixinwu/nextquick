/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * IWebHelper Description:
 * web helper�ӿ�
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-28   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/


using System.Net;

namespace NGP.Framework.Core
{
    /// <summary>
    /// web helper�ӿ�
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// ��ȡurl��Դ
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// ��HTTP�����Ļ�ȡIP��ַ
        /// </summary>
        /// <returns>IP��ַ�ַ���</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// ��ȡ����url
        /// </summary>
        /// <param name="includeQueryString">ָʾ�Ƿ������ѯ�ַ�����ֵ</param>
        /// <param name="useSsl">ָʾ�Ƿ��ȡSSL��ȫҳ��URL��ֵ�� ����null���Զ�ȷ��</param>
        /// <param name="lowercaseUrl">ָʾ�Ƿ�ΪСдURL��ֵ</param>
        /// <returns>URL</returns>
        string GetThisRequestUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ��ܱ���
        /// </summary>
        /// <returns>������ǰ�ȫ�ģ���Ϊ�棬����Ϊ��</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// ��ȡhost
        /// </summary>
        /// <param name="useSsl">�Ƿ��ȡSSL��ȫURL</param>
        /// <returns>hostλ��</returns>
        string GetStoreHost(bool useSsl);

        /// <summary>
        /// ��ȡλ��
        /// </summary>
        /// <param name="useSsl">�Ƿ��ȡSSL��ȫURL; ����null���Զ�ȷ��</param>
        /// <returns>location</returns>
        string GetStoreLocation(bool? useSsl = null);

        /// <summary>
        /// �޸�URL�Ĳ�ѯ�ַ���
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="key">��ѯҪ��ӵĲ�����</param>
        /// <param name="values">��ѯҪ��ӵĲ���ֵ</param>
        /// <returns>���ݲ�ѯ��������URL</returns>
        string ModifyQueryString(string url, string key, params string[] values);

        /// <summary>
        /// ��URL��ɾ����ѯ����
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="key">��ѯҪ�Ƴ��Ĳ�����</param>
        /// <param name="value">��ѯҪ�Ƴ��Ĳ���ֵ</param>
        /// <returns>û�д��ݲ�ѯ��������URL</returns>
        string RemoveQueryString(string url, string key, string value = null);

        /// <summary>
        /// �Ƿ�ָ����IP��ַ
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>Result</returns>
        bool IsIpAddressSet(IPAddress address);

        /// <summary>
        /// �����ƻ�ȡ��ѯ�ַ���ֵ
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// ����Ӧ�ó�����
        /// </summary>
        /// <param name="makeRedirect">һ��ֵ��ָʾ�����Ƿ�Ӧ������������������ض���</param>
        void RestartAppDomain(bool makeRedirect = false);

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�Ƿ񽫿ͻ����ض�����λ��
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// ��ȡ��ǰ��HTTP����Э��
        /// </summary>
        string CurrentRequestProtocol { get; }
    }
}

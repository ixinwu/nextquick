/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * WebHelper Description:
 * web helper
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-28   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NGP.Framework.Core;

namespace NGP.Framework.WebApi.Core
{
    /// <summary>
    /// web helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        #region Fields 

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INGPFileProvider _fileProvider;

        #endregion

        #region Ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="fileProvider"></param>
        public WebHelper(
            IHttpContextAccessor httpContextAccessor,
            INGPFileProvider fileProvider)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// ��鵱ǰ��HTTP�����Ƿ����
        /// </summary>
        /// <returns>���������Ϊ��; �����Ǽٵ�</returns>
        protected virtual bool IsRequestAvailable()
        {
            if (_httpContextAccessor?.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// д��web.config
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                _fileProvider.SetLastWriteTimeUtc(_fileProvider.MapPath(NGPInfrastructureDefaults.WebConfigPath), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ��ȡurl��Դ
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            // ��ĳЩ����£�URL referrerΪnull�����磬��IE 8�У�
            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        /// <summary>
        /// ��HTTP�����Ļ�ȡIP��ַ
        /// </summary>
        /// <returns>IP��ַ�ַ���</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            try
            {
                // ���ȳ��Դ�ת���ı�ͷ�л�ȡIP��ַ
                if (_httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    // X-Forwarded-For��XFF��HTTPͷ�ֶ�������ʶ��ͨ��HTTP������ؾ��������ӵ�Web�������Ŀͻ��˵�ԭʼIP��ַ����ʵ��׼
                    var forwardedHttpHeaderKey = "X-FORWARDED-FOR";                    

                    var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers[forwardedHttpHeaderKey];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        result = forwardedHeader.FirstOrDefault();
                }

                // ����˱�ͷ�����ڣ��볢�Ի�ȡ����Զ��IP��ַ
                if (string.IsNullOrEmpty(result) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    result = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            // һЩ��֤
            if (result != null && result.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                result = IPAddress.Loopback.ToString();

            // ��TryParse����֧�ִ��˿ںŵ�IPv4
            if (IPAddress.TryParse(result ?? string.Empty, out var ip))
                // IP��ַ��Ч 
                result = ip.ToString();
            else if (!string.IsNullOrEmpty(result))
                // ɾ���˿�
                result = result.Split(':').FirstOrDefault();

            return result;
        }

        /// <summary>
        /// ��ȡ����url
        /// </summary>
        /// <param name="includeQueryString">ָʾ�Ƿ������ѯ�ַ�����ֵ</param>
        /// <param name="useSsl">ָʾ�Ƿ��ȡSSL��ȫҳ��URL��ֵ�� ����null���Զ�ȷ��</param>
        /// <param name="lowercaseUrl">ָʾ�Ƿ�ΪСдURL��ֵ</param>
        /// <returns>URL</returns>
        public virtual string GetThisRequestUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            // ��ȡλ��
            var storeLocation = GetStoreLocation(useSsl ?? IsCurrentConnectionSecured());

            // ���URL�ı���·��
            var pageUrl = $"{storeLocation.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

            // ����ѯ�ַ�����ӵ�URL
            if (includeQueryString)
                pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

            // �Ƿ�URLת��ΪСд
            if (lowercaseUrl)
                pageUrl = pageUrl.ToLowerInvariant();

            return pageUrl;
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ��ܱ���
        /// </summary>
        /// <returns>������ǰ�ȫ�ģ���Ϊ�棬����Ϊ��</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        /// <summary>
        /// ��ȡhost
        /// </summary>
        /// <param name="useSsl">�Ƿ��ȡSSL��ȫURL</param>
        /// <returns>hostλ��</returns>
        public virtual string GetStoreHost(bool useSsl)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            // ���Դ�����HOSTͷ��ȡ
            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (StringValues.IsNullOrEmpty(hostHeader))
                return string.Empty;

            // ��������ӵ�URL
            var storeHost = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader.FirstOrDefault()}";

            // ȷ����б�ܽ���
            storeHost = $"{storeHost.TrimEnd('/')}/";

            return storeHost;
        }

        /// <summary>
        /// �Ƿ�ָ����IP��ַ
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>Result</returns>
        public virtual bool IsIpAddressSet(IPAddress address)
        {
            return address != null && address.ToString() != IPAddress.IPv6Loopback.ToString();
        }

        /// <summary>
        /// ��ȡλ��
        /// </summary>
        /// <param name="useSsl">�Ƿ��ȡSSL��ȫURL; ����null���Զ�ȷ��</param>
        /// <returns>location</returns>
        public virtual string GetStoreLocation(bool? useSsl = null)
        {
            var storeLocation = string.Empty;

            // ��ȡhost
            var storeHost = GetStoreHost(useSsl ?? IsCurrentConnectionSecured());
            if (!string.IsNullOrEmpty(storeHost))
            {
                // ������ڣ����Ӧ�ó���·����
                storeLocation = IsRequestAvailable() ? $"{storeHost.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.PathBase}" : storeHost;
            }

            // ȷ����б�ܽ���
            storeLocation = $"{storeLocation.TrimEnd('/')}/";

            return storeLocation;
        }

        /// <summary>
        /// �޸�URL�Ĳ�ѯ�ַ���
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="key">��ѯҪ��ӵĲ�����</param>
        /// <param name="values">��ѯҪ��ӵĲ���ֵ</param>
        /// <returns>���ݲ�ѯ��������URL</returns>
        public virtual string ModifyQueryString(string url, string key, params string[] values)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                return url;

            // ��ȡ��ǰ����
            var uri = new Uri(url);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query);

            // ��Ӳ���
            queryParameters[key] = new StringValues(values);
            var queryBuilder = new QueryBuilder(queryParameters
                .ToDictionary(parameter => parameter.Key, parameter => parameter.Value.ToString()));

            // ʹ�ô��ݵĲ�ѯ����������URL
            url = $"{uri.GetLeftPart(UriPartial.Path)}{queryBuilder.ToQueryString()}{uri.Fragment}";

            return url;
        }

        /// <summary>
        /// ��URL��ɾ����ѯ����
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="key">��ѯҪ�Ƴ��Ĳ�����</param>
        /// <param name="value">��ѯҪ�Ƴ��Ĳ���ֵ</param>
        /// <returns>û�д��ݲ�ѯ��������URL</returns>
        public virtual string RemoveQueryString(string url, string key, string value = null)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (string.IsNullOrEmpty(key))
                return url;

            // ��ȡ��ǰ����
            var uri = new Uri(url);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query)
                .SelectMany(parameter => parameter.Value, (parameter, queryValue) => new KeyValuePair<string, string>(parameter.Key, queryValue))
                .ToList();

            if (!string.IsNullOrEmpty(value))
            {
                // ����Ѵ��ݣ���ɾ���ض��Ĳ�ѯ����ֵ
                queryParameters.RemoveAll(parameter => parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                    && parameter.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                // ��ͨ����ɾ����ѯ����
                queryParameters.RemoveAll(parameter => parameter.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }

            // �����µ�URL�������ݲ�ѯ����
            url = $"{uri.GetLeftPart(UriPartial.Path)}{new QueryBuilder(queryParameters).ToQueryString()}{uri.Fragment}";

            return url;
        }

        /// <summary>
        /// �����ƻ�ȡ��ѯ�ַ���ֵ
        /// </summary>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <param name="name">Query parameter name</param>
        /// <returns>Query string value</returns>
        public virtual T QueryString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
                return default(T);

            return _httpContextAccessor.HttpContext.Request.Query[name].ToString().To<T>();
        }

        /// <summary>
        /// ����Ӧ�ó�����
        /// </summary>
        /// <param name="makeRedirect">һ��ֵ��ָʾ�����Ƿ�Ӧ������������������ض���</param>
        public virtual void RestartAppDomain(bool makeRedirect = false)
        {
            //the site will be restarted during the next request automatically
            //_applicationLifetime.StopApplication();

            //"touch" web.config to force restart
            var success = TryWriteWebConfig();
            if (!success)
            {
                throw new NGPException("nopCommerce needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                    "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                    "- run the application in a full trust environment, or" + Environment.NewLine +
                    "- give the application write access to the 'web.config' file.");
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�Ƿ񽫿ͻ����ض�����λ��
        /// </summary>
        public virtual bool IsRequestBeingRedirected
        {
            get
            {
                var response = _httpContextAccessor.HttpContext.Response;
                //ASP.NET 4 style - return response.IsRequestBeingRedirected;
                int[] redirectionStatusCodes = { StatusCodes.Status301MovedPermanently, StatusCodes.Status302Found };
                return redirectionStatusCodes.Contains(response.StatusCode);
            }
        }

        /// <summary>
        /// ��ȡ��ǰ��HTTP����Э��
        /// </summary>
        public virtual string CurrentRequestProtocol => IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        #endregion
    }
}
/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * IProcessCommand Description:
 * ��������ӿ�--ÿ�����������������¸����������
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-3-20   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/


using System;

namespace NGP.Framework.Core
{
    /// <summary>
    /// ��������ӿ�--ÿ�����������������¸����������
    /// </summary>
    /// <typeparam name="TContext">ͨ�ò���</typeparam>
    public interface IProcessCommand<TContext>
    {
        /// <summary>
        /// �����һ����
        /// </summary>
        /// <param name="step">��һ����</param>
        void AddNextStep(IProcessCommand<TContext> step);

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="request">�������</param>
        /// <param name="ctx">�����Ĳ���</param>
        /// <returns>���ش�����</returns>
        INGPResponse HandleProcess(INGPRequest request,TContext ctx);
    }
}

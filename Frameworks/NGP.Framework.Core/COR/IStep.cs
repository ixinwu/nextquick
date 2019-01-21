/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * IStep Description:
 * ְ����ģʽ��ÿ��ְ�𣨴����裩��Ҫ�̳еĽӿ�
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using System;

namespace NGP.Framework.Core
{
    /// <summary>
    /// ְ����ģʽ��ÿ��ְ�𣨴����裩��Ҫ�̳еĽӿ�
    /// </summary>
    /// <typeparam name="TContext">����������</typeparam>
    public interface IStep<TContext>
    {
        /// <summary>
        /// �����һ����
        /// </summary>
        /// <param name="step">��һ����</param>
        /// <param name="resultFor">�������һ����������һ���ɹ���ʧ�ܺ�ִ��</param>
        /// <returns>��һ����</returns>
        IStep<TContext> AddNextStep(IStep<TContext> step, bool resultFor = true);
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="ctx">����������</param>
        void HandleProcess(TContext ctx);

        /// <summary>
        /// ���账������ص�
        /// </summary>
        EventHandler<Tuple<TContext, bool>> OnStepComplete { get; set; }
    }
}

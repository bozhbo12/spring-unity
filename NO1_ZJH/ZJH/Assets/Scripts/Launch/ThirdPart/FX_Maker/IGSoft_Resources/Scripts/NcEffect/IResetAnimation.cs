/// <summary>
/// ��Ч���³�ʹ���ӿ�
/// </summary>
public interface IResetAnimation
{
    /// <summary>
    /// ���ö��� 
    /// ���ڵ�ʹ��ncDelay�ӳ�,�ӽڵ�Ҳ�����ӳ�ʱ,ͳһ�������ýӿڻᷢ��ncDelay�ӳ�ʧЧ����
    /// ԭ������ͳһ���ýӿ��л������ӽڵ㣬��ʱ�ӽڵ��Ѿ���ʼ����ʱ������ncdelayʧЧ
    /// ͳһ��������ncDelay���ӳ٣���nc����ӳٺ��ٵ����ӽڵ��ӳ�
    /// </summary>
    void ResetAnimation();

    /// <summary>
    /// ���ø������Ƿ�nc
    /// </summary>
    /// <param name="bValue"></param>
    void SetParentIsNcDelay(bool bValue);

    /// <summary>
    /// ��ȡ�������Ƿ���nc
    /// </summary>
    /// <returns></returns>
    bool GetParentIsNcDelay();
}
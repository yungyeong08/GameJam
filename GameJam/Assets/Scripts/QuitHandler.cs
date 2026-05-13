using UnityEngine;

public class QuitHandler : MonoBehaviour
{
    public void ExitGame()
    {
        // ���� ����� ������ ������
        Application.Quit();

        // ����Ƽ ������ �󿡼� �׽�Ʈ�� �� ������ �� (���� �Ŀ��� �������)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
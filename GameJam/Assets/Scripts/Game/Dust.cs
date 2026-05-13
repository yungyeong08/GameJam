using UnityEngine;

public class Dust : MonoBehaviour
{
    [Header("Settings")]
    public int dustType; // 먼지의 종류(레벨)
    public bool hasMerged = false; // 병합 중복 체크용

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 이미 병합 로직이 실행 중이라면 리턴
        if (hasMerged)
            return;

        // 충돌한 상대방에게서 Dust 컴포넌트 추출
        Dust otherDust = collision.gameObject.GetComponent<Dust>();

        // 상대방이 존재하고 + 아직 병합 전이며 + 나랑 같은 타입일 때만 실행
        if (otherDust != null && !otherDust.hasMerged && otherDust.dustType == dustType)
        {
            // 1. 중복 방지를 위해 둘 다 병합 상태로 즉시 변경
            hasMerged = true;
            otherDust.hasMerged = true;

            // 2. 병합 위치 계산 (두 오브젝트의 중간)
            Vector3 mergePosition = (transform.position + otherDust.transform.position) / 2f;

            // 3. 게임 매니저(DustGame)를 찾아 다음 단계 생성 요청
            // ※ 만약 매니저 스크립트 이름이 여전히 FruitGame이라면 아래를 FruitGame으로 바꾸세요.
            DustGame gameManager = Object.FindAnyObjectByType<DustGame>();

            if (gameManager != null)
            {
                // MergeDusts 함수를 호출하여 다음 레벨 먼지 생성
                gameManager.MergeDusts(dustType, mergePosition);
            }

            // 4. 기존 먼지들 제거
            Destroy(otherDust.gameObject);
            Destroy(gameObject);
        }
    }
}
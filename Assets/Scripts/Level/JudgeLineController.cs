using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PigeonB1587.prpu
{
    public class JudgeLineController : MonoBehaviour
    {
        public GameObject judgmentLinePrefab;
        public LevelController levelController;
        public Transform jugdeLineFather;

        public List<JudgeLine> judgeLines = new List<JudgeLine>();
        public List<Sprite> judgeLineSprites = new List<Sprite>();

        public async UniTask SpawnJudgmentLine()
        {
            foreach (var item in GameInformation.Instance.levelStartInfo.judgeLineImages)
            {
                Debug.Log($"Load judgeLine sprite: {item.judgeLineIndex}");
                Sprite sprite = await GameInformation.Instance.LoadAddressableAsset<Sprite>(item.imageAddressableKey);
                judgeLineSprites.Add(sprite);
            }
            if (Reader.chart.judgeLineList.Length == 0)
                return;
            for (int i = 0; i < Reader.chart.judgeLineList.Length; i++)
            {
                GameObject lineObj = Instantiate(judgmentLinePrefab, jugdeLineFather);
                lineObj.name = $"JudgeLine ({i})";
                JudgeLine line = lineObj.GetComponent<JudgeLine>();
                judgeLines.Add(line);
                line.jugdeLineData = Reader.chart.judgeLineList[i];
                line.index = i;
                line.levelController = levelController;
            }
            for (int i = 0; i < judgeLines.Count; i++)
            {
                if (judgeLines[i].jugdeLineData.transform.fatherLineIndex != -1)
                    judgeLines[i].fatherLine = judgeLines[judgeLines[i].jugdeLineData.transform.fatherLineIndex].transform;
            }
            for (int i = 0; i < GameInformation.Instance.levelStartInfo.judgeLineImages.Count; i++)
            {
                var k = judgeLines[GameInformation.Instance.levelStartInfo.judgeLineImages[i].judgeLineIndex].GetComponent<SpriteRenderer>();
                k.sprite = judgeLineSprites[i];
                k.size = new Vector2(GameInformation.Instance.levelStartInfo.judgeLineImages[i].imageX, GameInformation.Instance.levelStartInfo.judgeLineImages[i].imageY);
            }
            judgeLines = SortJudgmentLine(judgeLines);

            await UniTask.CompletedTask;
            return;
        }

        public void Update()
        {
            if (!levelController.isLoading && !levelController.isOver)
            {
                UpdateJudgeLines();
            }
        }

        public void UpdateJudgeLines()
        {
            for (int i = 0; i < judgeLines.Count(); i++)
            {
                judgeLines[i].UpdateLine(levelController.time);
            }
            for (int i = 0; i < judgeLines.Count(); i++)
            {
                judgeLines[i].UpdateTransform();
            }
            for (int i = 0; i < judgeLines.Count(); i++)
            {
                judgeLines[i].UpdateNote();
            }
        }

        public List<JudgeLine> SortJudgmentLine(List<JudgeLine> lines)
        {
            if (lines == null || lines.Count == 0)
                return new List<JudgeLine>();

            int count = lines.Count;
            var children = new Dictionary<int, List<int>>();
            var roots = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int father = lines[i].jugdeLineData.transform.fatherLineIndex;

                if (father == -1 || father < 0 || father >= count)
                    roots.Add(i);
                else
                {
                    if (!children.ContainsKey(father))
                        children[father] = new List<int>();
                    children[father].Add(i);
                }
            }

            var sorted = new List<int>();
            var queue = new Queue<int>(roots);

            while (queue.Count > 0)
            {
                int curr = queue.Dequeue();
                sorted.Add(curr);

                if (children.TryGetValue(curr, out var kids))
                    foreach (var kid in kids)
                        queue.Enqueue(kid);
            }

            return sorted.Select(i => lines[i]).ToList();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;

public class LoveGaugeManager : MonoBehaviour
{
    public Image[] hearts; // ハート画像の配列
    public Sprite emptyHeart; // 空っぽのハート画像
    public Sprite fullHeart; // 赤いハート画像
    public Sprite hiddenHeart; // 非表示のハート画像

    public void UpdateHearts(int currentLove, int maxLove)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLove)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i < maxLove)
            {
                hearts[i].sprite = emptyHeart;
            }
            else
            {
                hearts[i].sprite = hiddenHeart;
            }
        }
    }
}

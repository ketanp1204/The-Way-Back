using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndingSequence : MonoBehaviour
{

    private GameSession gameSession;

    public string[] endingTexts = { "One day feels like the other. However, particularly this day has been so trivial, it kind of feels wasted. I feel tired and it’s late. I should go to bed.",
                                    "Before I had opened my eyes, I already lost my believe that this day might once turn out well. " + 
                                        "However, I proved myself wrong. I kind of enjoyed the time and I think it’s smart to make a plan of things I’d like to do in the next days. But nothing too complex. Babysteps!",
                                    "I should make a big red cross in today’s agenda! I feel surprisingly good and even though I’ve been living here for already quite a while now, it was super nice to explore my surrounding. " + 
                                        "It’s strange to live in a place which feels foreign and therefore I am glad, that I got to know it better, even though there are areas that I’d like to avoid. Disconnection is reasoned.",
                                    "It’s frightenin, to not know who you are today, even though you know who you’ve been your entire life. " + 
                                        "To forget – or even worse: to suppress – your true self, is the worst a person can do to itself. " + 
                                        "I am glad I’ve experienced all these things today. I feel bound and free at the same time. I feel disconnected and connected at the same time. " + 
                                        "I feel like a secret and the truth at the same time. " +
                                        "I feel like I’ve finally met a long lost friend in myself. Nonetheless, after all this time, I recognized this person immediately."};

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();

        int LOSAScore = GameSession.GetLOSA();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

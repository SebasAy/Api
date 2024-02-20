using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class ApiManager : MonoBehaviour
{
    public RawImage[] images;
    public TextMeshProUGUI[] CardText;
    public TextMeshProUGUI userText;

    private string fakeApiUrl = "https://my-json-server.typicode.com/SebasAy/Api";
    private string RickyMortyApiurl = "https://rickandmortyapi.com/api";
    private int currentUser = 1;

    private void Start()
    {
        SendRequest();
    }
    private void Update()
    {
        // Cambiar de usuario con las teclas de flecha izquierda y derecha
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextUser();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LastUser();
        }
        // Cambiar de usuario con las teclas 'A' y 'D'
        else if (Input.GetKeyDown(KeyCode.A))
        {
            LastUser();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            NextUser();
        }
    }
    public void SendRequest()
    {
        StartCoroutine(GetUserData(currentUser));
    }

    IEnumerator GetUserData(int uid)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/users/" + uid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

                userText.text = "User: " + user.username;

                int maxItems = Mathf.Min(user.deck.Length, Mathf.Min(images.Length, CardText.Length));

                for (int i = 0; i < maxItems; i++)
                {
                    StartCoroutine(GetCharacter(user.deck[i], i));
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator GetCharacter(int id, int itemIndex)
    {
        UnityWebRequest request = UnityWebRequest.Get(RickyMortyApiurl + "/character/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                CharacterData character = JsonUtility.FromJson<CharacterData>(request.downloadHandler.text);

                if (itemIndex < images.Length)
                {
                    StartCoroutine(DownloadImage(character.image, itemIndex));
                }

                CardText[itemIndex].text = character.name;
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator DownloadImage(string url, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            images[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public void NextUser()
    {
        if (currentUser < 3)
        {
            currentUser++;
            SendRequest();
        }
        else
        {
            currentUser = 1;
            SendRequest();
        }
    }

    public void LastUser()
    {
        if (currentUser > 1)
        {
            currentUser--;
            SendRequest();
        }
        else
        {
            currentUser = 3;
            SendRequest();
        }
    }


    [System.Serializable]
    public class UserData
    {
        public int id;
        public string username;
        public int[] deck;
    }
    [System.Serializable]
    public class CharacterData
    {
        public int id;
        public string name;
        public string image;
    }
}
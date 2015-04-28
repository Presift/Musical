using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureBlender : MonoBehaviour 
{
	public int matId;
	[System.Serializable]	
	public class textureData
	{
		public Color texColor = Color.white;
		public Texture texture;
	}
	public List<textureData> textureList;
	public bool useTextureList;
	public int useTextureId;
	public float blendTime = 1;
	public bool blend = false;
	public float blendValue = 0.0f;

	float timer = 0.0f;
	bool BlendUp = true;

	void Start() {

		GetComponent<Renderer>().sharedMaterials[matId].SetFloat("_Blend",0);
		if (textureList.Count < 2)
		{
			if (textureList.Count == 1)
			{
				textureList.RemoveAt(0);
			}
			textureData tdTemp = new textureData();
			textureList.Add(tdTemp);
			textureList[0].texture = GetComponent<Renderer>().sharedMaterials[matId].GetTexture("_MainTex");
			textureData tdTemp2 = new textureData();
			textureList.Add(tdTemp2);
			textureList[1].texture = GetComponent<Renderer>().sharedMaterials[matId].GetTexture("_Main2Tex");
		}
	}
	
	void Update() 
	{
		if( blend)
		{
			Blend();
		}
	}
	void Blend()
	{
		timer += Time.deltaTime;
		if(useTextureList)
		{
			if(useTextureId > textureList.Count -1 )
			{
				Debug.LogWarning ("Texture Id " + useTextureId + "does not exist in your Texture List");
				useTextureList = false;
			}
			else
			{
				if (BlendUp)
				{
					Texture tT = textureList[useTextureId].texture;
					Color tC = textureList[useTextureId].texColor;
					GetComponent<Renderer>().sharedMaterials[matId].SetTexture("_Main2Tex",tT);
					GetComponent<Renderer>().sharedMaterials[matId].SetColor("_Color2",tC);
				}
				else
				{
					Texture tT = textureList[useTextureId].texture;
					Color tC = textureList[useTextureId].texColor;
					GetComponent<Renderer>().sharedMaterials[matId].SetTexture("_MainTex",tT);
					GetComponent<Renderer>().sharedMaterials[matId].SetColor("_Color",tC);
				}
			}
		}

		if(blendValue < 1 & BlendUp)
		{
			blendValue = Mathf.Lerp(0,1,timer/blendTime);
			GetComponent<Renderer>().sharedMaterials[matId].SetFloat("_Blend",blendValue);
			if(timer > blendTime)
			{
				timer = 0.0f;
				blend = false;
				BlendUp = false;
			}
		}
		else
		{
			if(blendValue > 0 & BlendUp == false)
			{
				blendValue = Mathf.Lerp(1,0,timer/blendTime);
				GetComponent<Renderer>().sharedMaterials[matId].SetFloat("_Blend",blendValue);
			}
			if(timer > blendTime)
			{
				timer = 0.0f;
				blend = false;
				BlendUp = true;
			}
		}



	}
}


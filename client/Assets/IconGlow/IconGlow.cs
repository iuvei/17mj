using UnityEngine;
using System.Collections;

public class IconGlow : MonoBehaviour {

	public Texture icon;
	public RenderTexture iconGlowPing;
	public RenderTexture iconGlowPong;

	private Material blitMaterial;

	// Use this for initialization
	void Start () {

		blitMaterial = new Material (Shader.Find ("Hidden/SeperableBlur"));

		int width = icon.width / 2;
		int height = icon.height / 2;

		iconGlowPing = new RenderTexture( width, height, 0 );
		iconGlowPing.format = RenderTextureFormat.R8;
		iconGlowPing.wrapMode = TextureWrapMode.Clamp;

		iconGlowPong = new RenderTexture( width, height, 0 );
		iconGlowPong.format = RenderTextureFormat.R8;
		iconGlowPong.wrapMode = TextureWrapMode.Clamp;

		blitMaterial.SetFloat ("_SizeX", width);
		blitMaterial.SetFloat ("_SizeY", height);
		blitMaterial.SetFloat ("_BlurSpread", 1.0f);

		blitMaterial.SetVector ("_ChannelWeight", new Vector4 (0,0,0,1));
		blitMaterial.SetVector ("_BlurDir", new Vector4 (0,1,0,0));
		Graphics.Blit (icon, iconGlowPing, blitMaterial, 0 );
		blitMaterial.SetVector ("_ChannelWeight", new Vector4 (1,0,0,0));
		blitMaterial.SetVector ("_BlurDir", new Vector4(1,0,0,0));
		Graphics.Blit (iconGlowPing, iconGlowPong, blitMaterial, 0 );

		Material thisMaterial = this.GetComponent<Renderer>().sharedMaterial;
		thisMaterial.SetTexture ("_GlowTex", iconGlowPong);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

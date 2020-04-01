/* weixing8 2020.03.31
 */
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
	public AudioSource audioSource;
	string micDevice;
	int simpleFrequency = 16000;
	float startRecordTime, endRecordTime;



	// Start is called before the first frame update
	void Start()
	{
		string[] micDevices = Microphone.devices;
		foreach (string device in micDevices)
		{
			Debug.Log("Microphone.device " + device);
		}
		if (micDevices.Length > 0)
		{
			micDevice = micDevices[0];
		} else {
			Debug.LogWarning("maby no Microphone");
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	//开始录音
	public void StartRecording()
	{
		GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "松手结束录音";

		audioSource.clip = Microphone.Start(micDevice, false, 30, simpleFrequency);
		startRecordTime = Time.realtimeSinceStartup;
		Debug.Log("开始录音");
	}

	//停止录音
	public void StopRecording()
	{
		Microphone.End(micDevice);
		endRecordTime = Time.realtimeSinceStartup;
		Debug.Log("录音结束");
		GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "按住录音";

		//裁切掉尾部未录音部分
		if (endRecordTime - startRecordTime < audioSource.clip.length)
		{
			audioSource.clip = CutAudioClip(audioSource.clip, 0, endRecordTime - startRecordTime);
		}

		PlayRecord(audioSource.clip);
	}

	//播放录音
	public void PlayRecord(AudioClip clip)
	{
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
		Debug.Log("开始播放录音");
	}

	//裁切AudioClip
	public AudioClip CutAudioClip(AudioClip clip, float startTime, float endTime)
	{
		float[] data = new float[(int)(simpleFrequency * (endTime - startTime))];
		clip.GetData(data, (int)(simpleFrequency * startTime));
		AudioClip newClip = AudioClip.Create("newClip", data.Length, 1, simpleFrequency, false);
		newClip.SetData(data, 0);
		return newClip;
	}

	//获取声音Byte数组数据
	public byte[] GetClipData(AudioClip clip)
	{
		if (clip == null)
		{
			Debug.Log("GetClipData audio.clip is null");
			return null;
		}

		float[] samples = new float[clip.samples];
		clip.GetData(samples, 0);
		byte[] outData = new byte[samples.Length * 2];
		int rescaleFactor = 32767;
		for (int i = 0; i < samples.Length; i++)
		{
			short temshort = (short)(samples[i] * rescaleFactor);
			byte[] temdata = System.BitConverter.GetBytes(temshort);
			outData[i * 2] = temdata[0];
			outData[i * 2 + 1] = temdata[1];
		}
		if (outData == null || outData.Length <= 0)
		{
			Debug.Log("GetClipData intData is null");
			return null;
		}
		return outData;
	}
}

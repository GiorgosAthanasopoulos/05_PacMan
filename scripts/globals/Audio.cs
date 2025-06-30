using Godot;

namespace PacMan;

public partial class Audio : Node
{
	private static AudioStreamPlayer bgmPlayer = new();
	private static AudioStreamPlayer sfxPlayer = new();

	public static AudioStream RingtoneIntermission = (AudioStream)ResourceLoader.Load("res://assets/music/ringtone-intermission/pacman_ringtone_interlude.mp3");
	public static AudioStream RingtoneIntro = (AudioStream)ResourceLoader.Load("res://assets/music/ringtone-intro/pacman_ringtone.mp3");

	public static AudioStream Intro = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-beginning/pacman_beginning.wav");
	public static AudioStream Chomp = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-chomp/pacman_chomp.wav");
	public static AudioStream Death = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-death/pacman_death.wav");
	public static AudioStream EatFruit = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-eatfruit/pacman_eatfruit.wav");
	public static AudioStream EatGhost = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-eatghost/pacman_eatghost.wav");
	public static AudioStream ExtraPac = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-extrapac/pacman_extrapac.wav");
	public static AudioStream Intermission = (AudioStream)ResourceLoader.Load("res://assets/sounds/pacman-intermission/pacman_intermission.wav");

	public override void _Ready()
	{
		bgmPlayer.Bus = "Music";
		AddChild(bgmPlayer);

		sfxPlayer.Bus = "Sound";
		AddChild(sfxPlayer);

		Settings.LoadSettings();
		SetMasterVolume(Settings.MasterVolume);
		SetMusicVolume(Settings.MusicVolume);
		SetSoundVolume(Settings.SoundVolume);
	}

	public static void PlayBGM(AudioStream p_bgm)
	{
		if (bgmPlayer.Playing)
			bgmPlayer.Stop();

		bgmPlayer.Stream = p_bgm;
		bgmPlayer.Play();
	}

	public static void PlaySFX(AudioStream p_sfx, bool only = false)
	{
		if (only && sfxPlayer.Playing)
			sfxPlayer.Stop();

		sfxPlayer.Stream = p_sfx;
		sfxPlayer.Play();
	}

	public static bool IsPlaying(AudioStream p_stream)
	{
		return sfxPlayer.Stream == p_stream && sfxPlayer.Playing;
	}

	public static void SetMasterVolume(float p_volume)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), p_volume);

		Settings.MasterVolume = p_volume;
		Settings.SaveSettings();
	}

	public static void SetMusicVolume(float p_volume)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), p_volume);

		Settings.MusicVolume = p_volume;
		Settings.SaveSettings();
	}

	public static void SetSoundVolume(float p_volume)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Sound"), p_volume);

		Settings.SoundVolume = p_volume;
		Settings.SaveSettings();
	}

	public static void StopAllAudio()
	{
		if (bgmPlayer.Playing)
			bgmPlayer.Stop();

		if (sfxPlayer.Playing)
			sfxPlayer.Stop();
	}
}

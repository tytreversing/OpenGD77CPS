namespace DMR;

public class OpenGD77CommsTransferData
{
	public enum CommsDataMode
	{
		DataModeNone,
		DataModeReadFlash,
		DataModeReadEEPROM,
		DataModeWriteFlash,
		DataModeWriteEEPROM,
		DataModeReadMCUROM,
		DataModeReadScreenGrab,
		DataModeWriteWAV,
		DataModeReadAMBE,
		DataModeReadRadioInfo,
		DataModeReadSecureRegisters,
		DataModeReadSettings,
		DataModeWriteSettings
	}

	public enum CommsAction
	{
		NONE,
		BACKUP_EEPROM,
		RESTORE_EEPROM,
		BACKUP_FLASH,
		RESTORE_FLASH,
		BACKUP_CALIBRATION,
		RESTORE_CALIBRATION,
		READ_CODEPLUG,
		WRITE_CODEPLUG,
		BACKUP_MCU_ROM,
		DOWNLOAD_SCREENGRAB,
		COMPRESS_AUDIO,
		WRITE_VOICE_PROMPTS,
		WRITE_SATELLITE_KEPS,
		BACKUP_SETTINGS,
		RESTORE_SETTINGS,
		READ_THEME,
		WRITE_THEME,
		READ_SECURE_REGISTERS,
		SAVE_NMEA_LOG,
		READ_SETTINGS,
		WRITE_SETTINGS
	}

	public CommsDataMode mode;

	public CommsAction action;

	public int startDataAddressInTheRadio;

	public int transferLength;

	public int localDataBufferStartPosition;

	public int data_sector;

	public byte[] dataBuff;

	public int responseCode;

	public OpenGD77CommsTransferData(CommsAction theAction = CommsAction.NONE)
	{
		action = theAction;
	}
}

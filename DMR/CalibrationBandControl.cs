using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DMR;

public class CalibrationBandControl : UserControl
{
	private CalibrationData _calibrationData;

	private string _type = "";

	private IContainer components;

	private GroupBox grpSquelch;

	private NumericUpDown nudSquelchNarrowTightClose;

	private NumericUpDown nudSquelchWideTightClose;

	private NumericUpDown nudSquelchNarrowTightOpen;

	private NumericUpDown nudSquelchWideTightOpen;

	private Label lblSquelchCloseTight;

	private Label lblSquelchOpenTight;

	private NumericUpDown nudSquelchNarrowNormClose;

	private NumericUpDown nudSquelchWideNormClose;

	private NumericUpDown nudSquelchNarrowNormOpen;

	private NumericUpDown nudSquelchWideNormOpen;

	private Label lblSquelchNarrow;

	private Label lblSquelchWide;

	private Label lblSquelchCloseNormal;

	private Label lblSquelchOpenNormal;

	private NumericUpDown nudVhfOscRef;

	private Label lblReferenceOscTuning;

	private Label lblReceiveAGCTarget;

	private NumericUpDown nudReceiveAGCTarget;

	private Label lblAnalogMicGain;

	private NumericUpDown nudAnalogMicGain;

	private CalibrationPowerControl calibrationPowerControlHigh;

	private CalibrationPowerControl calibrationPowerControlLow;

	private CalibrationPowerControl calibrationTXIandQ;

	private GroupBox grpSMeter;

	private NumericUpDown nudSMeterHigh;

	private NumericUpDown nudSMeterLow;

	private Label lblSMeterLowEnd;

	private Label lblSMeterHighEnd;

	private GroupBox grpAnalogTxDeviation;

	private GroupBox grpIFGain;

	private Label lblDigitalGainNarrow;

	private Label lblDigitalGainWide;

	private NumericUpDown nudDigitalRxGainNarrowband;

	private NumericUpDown nudDigitalRxGainWideband;

	private Label lblDigitalGainRx;

	private NumericUpDown nudAnalogTxDeviationDCSNarrowband;

	private Label lblCTCSS;

	private NumericUpDown nudlAnalogTxDeviationDCSWideband;

	private Label lblDCS;

	private NumericUpDown nudAnalogTxDeviationCTCSSNarrowband;

	private Label lblAnalogTxDeviationNarrow;

	private NumericUpDown nudAnalogTxDeviationCTCSSWideband;

	private Label lblAnalogTxDeviationWide;

	private NumericUpDown nudAnalogTxDeviation1750Tone;

	private Label lblToneBurst;

	private NumericUpDown nudAnalogTxDeviationDTMF;

	private Label lblDTMF;

	private GroupBox grpAnalogGain;

	private Label lblAnalogAudioGainNarrow;

	private Label lblAnalogAudioGainWide;

	private NumericUpDown nudAnalogRxGainNarrowband;

	private NumericUpDown nudAnalogRxGainWideband;

	private Label lblAnalogAudioGainRx;

	private Label lblOverallGain;

	private NumericUpDown nudAnalogTxGainNarrowband;

	private NumericUpDown nudAnalogTxGainWideband;

	public string Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
			string[] names = new string[8] { "136MHz", "140MHz", "145MHz", "150MHz", "155MHz", "160MHz", "165MHz", "172MHz" };
			string[] names2 = new string[16]
			{
				"400MHz", "405MHz", "410MHz", "415MHz", "420MHz", "425MHz", "430MHz", "435MHz", "440MHz", "445MHz",
				"450MHz", "455MHz", "460MHz", "465MHz", "470MHz", "475MHz"
			};
			string[] names3 = new string[8] { "405MHz", "415MHz", "425MHz", "435MHz", "445MHz", "455MHz", "465MHz", "475MHz" };
			string type = _type;
			if (!(type == "VHF"))
			{
				if (type == "UHF")
				{
					calibrationPowerControlHigh.Cols = 16;
					calibrationPowerControlLow.Cols = 16;
					calibrationPowerControlLow.Names = names2;
					calibrationPowerControlHigh.Names = names2;
					calibrationTXIandQ.Names = names3;
				}
			}
			else
			{
				calibrationPowerControlHigh.Cols = 8;
				calibrationPowerControlLow.Cols = 8;
				calibrationPowerControlLow.Names = names;
				calibrationPowerControlHigh.Names = names;
				calibrationTXIandQ.Names = names;
			}
		}
	}

	public CalibrationData data
	{
		get
		{
			_calibrationData.DACOscRefTune = (short)nudVhfOscRef.Value;
			_calibrationData.MuteNormalWidebandOpen1 = (byte)nudSquelchWideNormOpen.Value;
			_calibrationData.MuteNormalWidebandClose1 = (byte)nudSquelchWideNormClose.Value;
			_calibrationData.MuteStrictWidebandOpen1 = (byte)nudSquelchWideTightOpen.Value;
			_calibrationData.MuteStrictWidebandClose1 = (byte)nudSquelchWideTightClose.Value;
			_calibrationData.MuteNormalNarrowbandOpen1 = (byte)nudSquelchNarrowNormOpen.Value;
			_calibrationData.MuteNormalNarrowbandClose1 = (byte)nudSquelchNarrowNormClose.Value;
			_calibrationData.MuteStrictNarrowbandOpen1 = (byte)nudSquelchNarrowTightOpen.Value;
			_calibrationData.MuteStrictNarrowbandClose1 = (byte)nudSquelchNarrowTightClose.Value;
			_calibrationData.ReceiveAGCGainTarget = (byte)nudReceiveAGCTarget.Value;
			_calibrationData.AnalogMicGain = (byte)nudAnalogMicGain.Value;
			_calibrationData.RSSILowerThreshold = (byte)nudSMeterLow.Value;
			_calibrationData.RSSIUpperThreshold = (byte)nudSMeterHigh.Value;
			_calibrationData.MuteStrictWidebandOpen2 = _calibrationData.MuteStrictWidebandOpen1;
			_calibrationData.MuteStrictWidebandClose2 = _calibrationData.MuteStrictWidebandClose1;
			_calibrationData.MuteStrictNarrowbandOpen2 = _calibrationData.MuteStrictNarrowbandOpen1;
			_calibrationData.MuteStrictNarrowbandClose2 = _calibrationData.MuteStrictNarrowbandClose1;
			_calibrationData.AnalogTxDeviationDTMF = (byte)nudAnalogTxDeviationDTMF.Value;
			_calibrationData.AnalogTxDeviation1750Toneburst = (byte)nudAnalogTxDeviation1750Tone.Value;
			_calibrationData.AnalogTxDeviationCTCSSWideband = (byte)nudAnalogTxDeviationCTCSSWideband.Value;
			_calibrationData.AnalogTxDeviationCTCSSNarrowband = (byte)nudAnalogTxDeviationCTCSSNarrowband.Value;
			_calibrationData.AnalogTxDeviationDCSWideband = (byte)nudlAnalogTxDeviationDCSWideband.Value;
			_calibrationData.AnalogTxDeviationDCSNarrowband = (byte)nudAnalogTxDeviationDCSNarrowband.Value;
			_calibrationData.DigitalRxGainWideband_NOTCONFIRMED = (ushort)nudDigitalRxGainWideband.Value;
			_calibrationData.DigitalRxGainNarrowband_NOTCONFIRMED = (ushort)nudDigitalRxGainNarrowband.Value;
			_calibrationData.AnalogTxOverallDeviationWideband = (ushort)nudAnalogTxGainWideband.Value;
			_calibrationData.AnalogTxOverallDeviationNarrband = (ushort)nudAnalogTxGainNarrowband.Value;
			_calibrationData.AnalogRxAudioGainWideband = (byte)nudAnalogRxGainWideband.Value;
			_calibrationData.AnalogRxAudioGainNarrowband = (byte)nudAnalogRxGainNarrowband.Value;
			int num = calibrationPowerControlLow.Rows * calibrationPowerControlLow.Cols;
			for (int i = 0; i < num; i++)
			{
				_calibrationData.PowerSettings[i].lowPower = (byte)calibrationPowerControlLow.Values[i];
				_calibrationData.PowerSettings[i].highPower = (byte)calibrationPowerControlHigh.Values[i];
			}
			num = calibrationTXIandQ.Rows * calibrationTXIandQ.Cols;
			for (int j = 0; j < num; j++)
			{
				_calibrationData.Dmr4FskDeviation[j] = (byte)calibrationTXIandQ.Values[j];
			}
			return _calibrationData;
		}
		set
		{
			_calibrationData = value;
			nudVhfOscRef.Value = _calibrationData.DACOscRefTune;
			nudSquelchWideNormOpen.Value = _calibrationData.MuteNormalWidebandOpen1;
			nudSquelchWideNormClose.Value = _calibrationData.MuteNormalWidebandClose1;
			nudSquelchWideTightOpen.Value = _calibrationData.MuteStrictWidebandOpen1;
			nudSquelchWideTightClose.Value = _calibrationData.MuteStrictWidebandClose1;
			nudSquelchNarrowNormOpen.Value = _calibrationData.MuteNormalNarrowbandOpen1;
			nudSquelchNarrowNormClose.Value = _calibrationData.MuteNormalNarrowbandClose1;
			nudSquelchNarrowTightOpen.Value = _calibrationData.MuteStrictNarrowbandOpen1;
			nudSquelchNarrowTightClose.Value = _calibrationData.MuteStrictNarrowbandClose1;
			nudReceiveAGCTarget.Value = _calibrationData.ReceiveAGCGainTarget;
			nudAnalogMicGain.Value = _calibrationData.AnalogMicGain;
			nudSMeterLow.Value = _calibrationData.RSSILowerThreshold;
			nudSMeterHigh.Value = _calibrationData.RSSIUpperThreshold;
			nudAnalogTxDeviationDTMF.Value = _calibrationData.AnalogTxDeviationDTMF;
			nudAnalogTxDeviation1750Tone.Value = _calibrationData.AnalogTxDeviation1750Toneburst;
			nudAnalogTxDeviationCTCSSWideband.Value = _calibrationData.AnalogTxDeviationCTCSSWideband;
			nudAnalogTxDeviationCTCSSNarrowband.Value = _calibrationData.AnalogTxDeviationCTCSSNarrowband;
			nudlAnalogTxDeviationDCSWideband.Value = _calibrationData.AnalogTxDeviationDCSWideband;
			nudAnalogTxDeviationDCSNarrowband.Value = _calibrationData.AnalogTxDeviationDCSNarrowband;
			nudDigitalRxGainWideband.Value = _calibrationData.DigitalRxGainWideband_NOTCONFIRMED;
			nudDigitalRxGainNarrowband.Value = _calibrationData.DigitalRxGainNarrowband_NOTCONFIRMED;
			nudAnalogTxGainWideband.Value = _calibrationData.AnalogTxOverallDeviationWideband;
			nudAnalogTxGainNarrowband.Value = _calibrationData.AnalogTxOverallDeviationNarrband;
			nudAnalogRxGainWideband.Value = _calibrationData.AnalogRxAudioGainWideband;
			nudAnalogRxGainNarrowband.Value = _calibrationData.AnalogRxAudioGainNarrowband;
			int num = calibrationPowerControlLow.Rows * calibrationPowerControlLow.Cols;
			int[] array = new int[num];
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = _calibrationData.PowerSettings[i].lowPower;
				array2[i] = _calibrationData.PowerSettings[i].highPower;
			}
			calibrationPowerControlLow.Values = array;
			calibrationPowerControlHigh.Values = array2;
			num = calibrationTXIandQ.Rows * calibrationTXIandQ.Cols;
			int[] array3 = new int[num];
			for (int j = 0; j < num; j++)
			{
				array3[j] = _calibrationData.Dmr4FskDeviation[j];
			}
			calibrationTXIandQ.Values = array3;
		}
	}

	public CalibrationBandControl()
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.grpSquelch = new System.Windows.Forms.GroupBox();
		this.nudSquelchNarrowTightClose = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchWideTightClose = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchNarrowTightOpen = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchWideTightOpen = new System.Windows.Forms.NumericUpDown();
		this.lblSquelchCloseTight = new System.Windows.Forms.Label();
		this.lblSquelchOpenTight = new System.Windows.Forms.Label();
		this.nudSquelchNarrowNormClose = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchWideNormClose = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchNarrowNormOpen = new System.Windows.Forms.NumericUpDown();
		this.nudSquelchWideNormOpen = new System.Windows.Forms.NumericUpDown();
		this.lblSquelchNarrow = new System.Windows.Forms.Label();
		this.lblSquelchWide = new System.Windows.Forms.Label();
		this.lblSquelchCloseNormal = new System.Windows.Forms.Label();
		this.lblSquelchOpenNormal = new System.Windows.Forms.Label();
		this.nudVhfOscRef = new System.Windows.Forms.NumericUpDown();
		this.lblReferenceOscTuning = new System.Windows.Forms.Label();
		this.lblReceiveAGCTarget = new System.Windows.Forms.Label();
		this.nudReceiveAGCTarget = new System.Windows.Forms.NumericUpDown();
		this.lblAnalogMicGain = new System.Windows.Forms.Label();
		this.nudAnalogMicGain = new System.Windows.Forms.NumericUpDown();
		this.grpSMeter = new System.Windows.Forms.GroupBox();
		this.nudSMeterHigh = new System.Windows.Forms.NumericUpDown();
		this.nudSMeterLow = new System.Windows.Forms.NumericUpDown();
		this.lblSMeterLowEnd = new System.Windows.Forms.Label();
		this.lblSMeterHighEnd = new System.Windows.Forms.Label();
		this.grpAnalogTxDeviation = new System.Windows.Forms.GroupBox();
		this.lblOverallGain = new System.Windows.Forms.Label();
		this.nudAnalogTxGainNarrowband = new System.Windows.Forms.NumericUpDown();
		this.nudAnalogTxGainWideband = new System.Windows.Forms.NumericUpDown();
		this.nudAnalogTxDeviationDCSNarrowband = new System.Windows.Forms.NumericUpDown();
		this.lblCTCSS = new System.Windows.Forms.Label();
		this.nudlAnalogTxDeviationDCSWideband = new System.Windows.Forms.NumericUpDown();
		this.lblDCS = new System.Windows.Forms.Label();
		this.nudAnalogTxDeviationCTCSSNarrowband = new System.Windows.Forms.NumericUpDown();
		this.lblAnalogTxDeviationNarrow = new System.Windows.Forms.Label();
		this.nudAnalogTxDeviationCTCSSWideband = new System.Windows.Forms.NumericUpDown();
		this.lblAnalogTxDeviationWide = new System.Windows.Forms.Label();
		this.nudAnalogTxDeviation1750Tone = new System.Windows.Forms.NumericUpDown();
		this.lblToneBurst = new System.Windows.Forms.Label();
		this.nudAnalogTxDeviationDTMF = new System.Windows.Forms.NumericUpDown();
		this.lblDTMF = new System.Windows.Forms.Label();
		this.grpIFGain = new System.Windows.Forms.GroupBox();
		this.lblDigitalGainNarrow = new System.Windows.Forms.Label();
		this.lblDigitalGainWide = new System.Windows.Forms.Label();
		this.nudDigitalRxGainNarrowband = new System.Windows.Forms.NumericUpDown();
		this.nudDigitalRxGainWideband = new System.Windows.Forms.NumericUpDown();
		this.lblDigitalGainRx = new System.Windows.Forms.Label();
		this.grpAnalogGain = new System.Windows.Forms.GroupBox();
		this.lblAnalogAudioGainNarrow = new System.Windows.Forms.Label();
		this.lblAnalogAudioGainWide = new System.Windows.Forms.Label();
		this.nudAnalogRxGainNarrowband = new System.Windows.Forms.NumericUpDown();
		this.nudAnalogRxGainWideband = new System.Windows.Forms.NumericUpDown();
		this.lblAnalogAudioGainRx = new System.Windows.Forms.Label();
		this.calibrationTXIandQ = new DMR.CalibrationPowerControl();
		this.calibrationPowerControlLow = new DMR.CalibrationPowerControl();
		this.calibrationPowerControlHigh = new DMR.CalibrationPowerControl();
		this.grpSquelch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowTightClose).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideTightClose).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowTightOpen).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideTightOpen).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowNormClose).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideNormClose).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowNormOpen).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideNormOpen).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudVhfOscRef).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudReceiveAGCTarget).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogMicGain).BeginInit();
		this.grpSMeter.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudSMeterHigh).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudSMeterLow).BeginInit();
		this.grpAnalogTxDeviation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxGainNarrowband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxGainWideband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationDCSNarrowband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudlAnalogTxDeviationDCSWideband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationCTCSSNarrowband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationCTCSSWideband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviation1750Tone).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationDTMF).BeginInit();
		this.grpIFGain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudDigitalRxGainNarrowband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudDigitalRxGainWideband).BeginInit();
		this.grpAnalogGain.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogRxGainNarrowband).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogRxGainWideband).BeginInit();
		base.SuspendLayout();
		this.grpSquelch.Controls.Add(this.nudSquelchNarrowTightClose);
		this.grpSquelch.Controls.Add(this.nudSquelchWideTightClose);
		this.grpSquelch.Controls.Add(this.nudSquelchNarrowTightOpen);
		this.grpSquelch.Controls.Add(this.nudSquelchWideTightOpen);
		this.grpSquelch.Controls.Add(this.lblSquelchCloseTight);
		this.grpSquelch.Controls.Add(this.lblSquelchOpenTight);
		this.grpSquelch.Controls.Add(this.nudSquelchNarrowNormClose);
		this.grpSquelch.Controls.Add(this.nudSquelchWideNormClose);
		this.grpSquelch.Controls.Add(this.nudSquelchNarrowNormOpen);
		this.grpSquelch.Controls.Add(this.nudSquelchWideNormOpen);
		this.grpSquelch.Controls.Add(this.lblSquelchNarrow);
		this.grpSquelch.Controls.Add(this.lblSquelchWide);
		this.grpSquelch.Controls.Add(this.lblSquelchCloseNormal);
		this.grpSquelch.Controls.Add(this.lblSquelchOpenNormal);
		this.grpSquelch.Location = new System.Drawing.Point(8, 273);
		this.grpSquelch.Name = "grpSquelch";
		this.grpSquelch.Size = new System.Drawing.Size(197, 144);
		this.grpSquelch.TabIndex = 8;
		this.grpSquelch.TabStop = false;
		this.grpSquelch.Text = "Analog squelch";
		this.nudSquelchNarrowTightClose.Location = new System.Drawing.Point(136, 112);
		this.nudSquelchNarrowTightClose.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchNarrowTightClose.Name = "nudSquelchNarrowTightClose";
		this.nudSquelchNarrowTightClose.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchNarrowTightClose.TabIndex = 14;
		this.nudSquelchWideTightClose.Location = new System.Drawing.Point(76, 112);
		this.nudSquelchWideTightClose.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchWideTightClose.Name = "nudSquelchWideTightClose";
		this.nudSquelchWideTightClose.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchWideTightClose.TabIndex = 13;
		this.nudSquelchNarrowTightOpen.Location = new System.Drawing.Point(136, 86);
		this.nudSquelchNarrowTightOpen.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchNarrowTightOpen.Name = "nudSquelchNarrowTightOpen";
		this.nudSquelchNarrowTightOpen.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchNarrowTightOpen.TabIndex = 12;
		this.nudSquelchWideTightOpen.Location = new System.Drawing.Point(76, 86);
		this.nudSquelchWideTightOpen.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchWideTightOpen.Name = "nudSquelchWideTightOpen";
		this.nudSquelchWideTightOpen.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchWideTightOpen.TabIndex = 11;
		this.lblSquelchCloseTight.AutoSize = true;
		this.lblSquelchCloseTight.Location = new System.Drawing.Point(6, 114);
		this.lblSquelchCloseTight.Name = "lblSquelchCloseTight";
		this.lblSquelchCloseTight.Size = new System.Drawing.Size(60, 13);
		this.lblSquelchCloseTight.TabIndex = 10;
		this.lblSquelchCloseTight.Text = "Close Tight";
		this.lblSquelchOpenTight.AutoSize = true;
		this.lblSquelchOpenTight.Location = new System.Drawing.Point(6, 88);
		this.lblSquelchOpenTight.Name = "lblSquelchOpenTight";
		this.lblSquelchOpenTight.Size = new System.Drawing.Size(60, 13);
		this.lblSquelchOpenTight.TabIndex = 9;
		this.lblSquelchOpenTight.Text = "Open Tight";
		this.nudSquelchNarrowNormClose.Location = new System.Drawing.Point(136, 58);
		this.nudSquelchNarrowNormClose.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchNarrowNormClose.Name = "nudSquelchNarrowNormClose";
		this.nudSquelchNarrowNormClose.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchNarrowNormClose.TabIndex = 8;
		this.nudSquelchWideNormClose.Location = new System.Drawing.Point(76, 58);
		this.nudSquelchWideNormClose.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchWideNormClose.Name = "nudSquelchWideNormClose";
		this.nudSquelchWideNormClose.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchWideNormClose.TabIndex = 7;
		this.nudSquelchNarrowNormOpen.Location = new System.Drawing.Point(136, 32);
		this.nudSquelchNarrowNormOpen.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchNarrowNormOpen.Name = "nudSquelchNarrowNormOpen";
		this.nudSquelchNarrowNormOpen.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchNarrowNormOpen.TabIndex = 6;
		this.nudSquelchWideNormOpen.Location = new System.Drawing.Point(76, 32);
		this.nudSquelchWideNormOpen.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSquelchWideNormOpen.Name = "nudSquelchWideNormOpen";
		this.nudSquelchWideNormOpen.Size = new System.Drawing.Size(41, 20);
		this.nudSquelchWideNormOpen.TabIndex = 5;
		this.lblSquelchNarrow.AutoSize = true;
		this.lblSquelchNarrow.Location = new System.Drawing.Point(136, 16);
		this.lblSquelchNarrow.Name = "lblSquelchNarrow";
		this.lblSquelchNarrow.Size = new System.Drawing.Size(41, 13);
		this.lblSquelchNarrow.TabIndex = 4;
		this.lblSquelchNarrow.Text = "Narrow";
		this.lblSquelchWide.AutoSize = true;
		this.lblSquelchWide.Location = new System.Drawing.Point(79, 16);
		this.lblSquelchWide.Name = "lblSquelchWide";
		this.lblSquelchWide.Size = new System.Drawing.Size(32, 13);
		this.lblSquelchWide.TabIndex = 3;
		this.lblSquelchWide.Text = "Wide";
		this.lblSquelchCloseNormal.AutoSize = true;
		this.lblSquelchCloseNormal.Location = new System.Drawing.Point(6, 60);
		this.lblSquelchCloseNormal.Name = "lblSquelchCloseNormal";
		this.lblSquelchCloseNormal.Size = new System.Drawing.Size(69, 13);
		this.lblSquelchCloseNormal.TabIndex = 2;
		this.lblSquelchCloseNormal.Text = "Close Normal";
		this.lblSquelchOpenNormal.AutoSize = true;
		this.lblSquelchOpenNormal.Location = new System.Drawing.Point(6, 34);
		this.lblSquelchOpenNormal.Name = "lblSquelchOpenNormal";
		this.lblSquelchOpenNormal.Size = new System.Drawing.Size(69, 13);
		this.lblSquelchOpenNormal.TabIndex = 1;
		this.lblSquelchOpenNormal.Text = "Open Normal";
		this.nudVhfOscRef.Location = new System.Drawing.Point(712, 322);
		this.nudVhfOscRef.Maximum = new decimal(new int[4] { 127, 0, 0, 0 });
		this.nudVhfOscRef.Minimum = new decimal(new int[4] { 128, 0, 0, -2147483648 });
		this.nudVhfOscRef.Name = "nudVhfOscRef";
		this.nudVhfOscRef.Size = new System.Drawing.Size(63, 20);
		this.nudVhfOscRef.TabIndex = 7;
		this.lblReferenceOscTuning.AutoSize = true;
		this.lblReferenceOscTuning.Location = new System.Drawing.Point(573, 322);
		this.lblReferenceOscTuning.Name = "lblReferenceOscTuning";
		this.lblReferenceOscTuning.Size = new System.Drawing.Size(133, 13);
		this.lblReferenceOscTuning.TabIndex = 6;
		this.lblReferenceOscTuning.Text = "Reference oscillator tuning\t";
		this.lblReceiveAGCTarget.AutoSize = true;
		this.lblReceiveAGCTarget.Location = new System.Drawing.Point(592, 354);
		this.lblReceiveAGCTarget.Name = "lblReceiveAGCTarget";
		this.lblReceiveAGCTarget.Size = new System.Drawing.Size(102, 13);
		this.lblReceiveAGCTarget.TabIndex = 6;
		this.lblReceiveAGCTarget.Text = "Receive AGC target\t";
		this.nudReceiveAGCTarget.Location = new System.Drawing.Point(711, 352);
		this.nudReceiveAGCTarget.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudReceiveAGCTarget.Name = "nudReceiveAGCTarget";
		this.nudReceiveAGCTarget.Size = new System.Drawing.Size(63, 20);
		this.nudReceiveAGCTarget.TabIndex = 7;
		this.lblAnalogMicGain.AutoSize = true;
		this.lblAnalogMicGain.Location = new System.Drawing.Point(11, 67);
		this.lblAnalogMicGain.Name = "lblAnalogMicGain";
		this.lblAnalogMicGain.Size = new System.Drawing.Size(82, 13);
		this.lblAnalogMicGain.TabIndex = 6;
		this.lblAnalogMicGain.Text = "Analog mic gain\t";
		this.nudAnalogMicGain.Location = new System.Drawing.Point(130, 65);
		this.nudAnalogMicGain.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogMicGain.Name = "nudAnalogMicGain";
		this.nudAnalogMicGain.Size = new System.Drawing.Size(63, 20);
		this.nudAnalogMicGain.TabIndex = 7;
		this.grpSMeter.Controls.Add(this.nudSMeterHigh);
		this.grpSMeter.Controls.Add(this.nudSMeterLow);
		this.grpSMeter.Controls.Add(this.lblSMeterLowEnd);
		this.grpSMeter.Controls.Add(this.lblSMeterHighEnd);
		this.grpSMeter.Location = new System.Drawing.Point(437, 324);
		this.grpSMeter.Name = "grpSMeter";
		this.grpSMeter.Size = new System.Drawing.Size(117, 87);
		this.grpSMeter.TabIndex = 10;
		this.grpSMeter.TabStop = false;
		this.grpSMeter.Text = "S Meter";
		this.nudSMeterHigh.Location = new System.Drawing.Point(61, 56);
		this.nudSMeterHigh.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSMeterHigh.Name = "nudSMeterHigh";
		this.nudSMeterHigh.Size = new System.Drawing.Size(41, 20);
		this.nudSMeterHigh.TabIndex = 16;
		this.nudSMeterLow.Location = new System.Drawing.Point(61, 28);
		this.nudSMeterLow.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudSMeterLow.Name = "nudSMeterLow";
		this.nudSMeterLow.Size = new System.Drawing.Size(41, 20);
		this.nudSMeterLow.TabIndex = 15;
		this.lblSMeterLowEnd.AutoSize = true;
		this.lblSMeterLowEnd.Location = new System.Drawing.Point(6, 30);
		this.lblSMeterLowEnd.Name = "lblSMeterLowEnd";
		this.lblSMeterLowEnd.Size = new System.Drawing.Size(49, 13);
		this.lblSMeterLowEnd.TabIndex = 8;
		this.lblSMeterLowEnd.Text = "Low End\t";
		this.lblSMeterHighEnd.AutoSize = true;
		this.lblSMeterHighEnd.Location = new System.Drawing.Point(6, 58);
		this.lblSMeterHighEnd.Name = "lblSMeterHighEnd";
		this.lblSMeterHighEnd.Size = new System.Drawing.Size(51, 13);
		this.lblSMeterHighEnd.TabIndex = 7;
		this.lblSMeterHighEnd.Text = "High End\t";
		this.grpAnalogTxDeviation.Controls.Add(this.lblOverallGain);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxGainNarrowband);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxGainWideband);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxDeviationDCSNarrowband);
		this.grpAnalogTxDeviation.Controls.Add(this.lblCTCSS);
		this.grpAnalogTxDeviation.Controls.Add(this.nudlAnalogTxDeviationDCSWideband);
		this.grpAnalogTxDeviation.Controls.Add(this.lblDCS);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxDeviationCTCSSNarrowband);
		this.grpAnalogTxDeviation.Controls.Add(this.lblAnalogTxDeviationNarrow);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxDeviationCTCSSWideband);
		this.grpAnalogTxDeviation.Controls.Add(this.lblAnalogTxDeviationWide);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxDeviation1750Tone);
		this.grpAnalogTxDeviation.Controls.Add(this.lblToneBurst);
		this.grpAnalogTxDeviation.Controls.Add(this.nudAnalogTxDeviationDTMF);
		this.grpAnalogTxDeviation.Controls.Add(this.lblDTMF);
		this.grpAnalogTxDeviation.Location = new System.Drawing.Point(432, 183);
		this.grpAnalogTxDeviation.Name = "grpAnalogTxDeviation";
		this.grpAnalogTxDeviation.Size = new System.Drawing.Size(342, 130);
		this.grpAnalogTxDeviation.TabIndex = 11;
		this.grpAnalogTxDeviation.TabStop = false;
		this.grpAnalogTxDeviation.Text = "Analog Tx deviation";
		this.lblOverallGain.AutoSize = true;
		this.lblOverallGain.Location = new System.Drawing.Point(106, 85);
		this.lblOverallGain.Name = "lblOverallGain";
		this.lblOverallGain.Size = new System.Drawing.Size(63, 13);
		this.lblOverallGain.TabIndex = 11;
		this.lblOverallGain.Text = "Overall gain";
		this.nudAnalogTxGainNarrowband.Location = new System.Drawing.Point(264, 82);
		this.nudAnalogTxGainNarrowband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudAnalogTxGainNarrowband.Name = "nudAnalogTxGainNarrowband";
		this.nudAnalogTxGainNarrowband.Size = new System.Drawing.Size(65, 20);
		this.nudAnalogTxGainNarrowband.TabIndex = 9;
		this.nudAnalogTxGainWideband.Location = new System.Drawing.Point(177, 82);
		this.nudAnalogTxGainWideband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudAnalogTxGainWideband.Name = "nudAnalogTxGainWideband";
		this.nudAnalogTxGainWideband.Size = new System.Drawing.Size(65, 20);
		this.nudAnalogTxGainWideband.TabIndex = 10;
		this.nudAnalogTxDeviationDCSNarrowband.Location = new System.Drawing.Point(265, 53);
		this.nudAnalogTxDeviationDCSNarrowband.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogTxDeviationDCSNarrowband.Name = "nudAnalogTxDeviationDCSNarrowband";
		this.nudAnalogTxDeviationDCSNarrowband.Size = new System.Drawing.Size(47, 20);
		this.nudAnalogTxDeviationDCSNarrowband.TabIndex = 2;
		this.lblCTCSS.AutoSize = true;
		this.lblCTCSS.Location = new System.Drawing.Point(134, 34);
		this.lblCTCSS.Name = "lblCTCSS";
		this.lblCTCSS.Size = new System.Drawing.Size(42, 13);
		this.lblCTCSS.TabIndex = 1;
		this.lblCTCSS.Text = "CTCSS";
		this.nudlAnalogTxDeviationDCSWideband.Location = new System.Drawing.Point(178, 53);
		this.nudlAnalogTxDeviationDCSWideband.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudlAnalogTxDeviationDCSWideband.Name = "nudlAnalogTxDeviationDCSWideband";
		this.nudlAnalogTxDeviationDCSWideband.Size = new System.Drawing.Size(47, 20);
		this.nudlAnalogTxDeviationDCSWideband.TabIndex = 2;
		this.lblDCS.AutoSize = true;
		this.lblDCS.Location = new System.Drawing.Point(145, 55);
		this.lblDCS.Name = "lblDCS";
		this.lblDCS.Size = new System.Drawing.Size(29, 13);
		this.lblDCS.TabIndex = 1;
		this.lblDCS.Text = "DCS";
		this.nudAnalogTxDeviationCTCSSNarrowband.Location = new System.Drawing.Point(265, 28);
		this.nudAnalogTxDeviationCTCSSNarrowband.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogTxDeviationCTCSSNarrowband.Name = "nudAnalogTxDeviationCTCSSNarrowband";
		this.nudAnalogTxDeviationCTCSSNarrowband.Size = new System.Drawing.Size(47, 20);
		this.nudAnalogTxDeviationCTCSSNarrowband.TabIndex = 2;
		this.lblAnalogTxDeviationNarrow.AutoSize = true;
		this.lblAnalogTxDeviationNarrow.Location = new System.Drawing.Point(264, 12);
		this.lblAnalogTxDeviationNarrow.Name = "lblAnalogTxDeviationNarrow";
		this.lblAnalogTxDeviationNarrow.Size = new System.Drawing.Size(41, 13);
		this.lblAnalogTxDeviationNarrow.TabIndex = 1;
		this.lblAnalogTxDeviationNarrow.Text = "Narrow";
		this.nudAnalogTxDeviationCTCSSWideband.Location = new System.Drawing.Point(178, 28);
		this.nudAnalogTxDeviationCTCSSWideband.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogTxDeviationCTCSSWideband.Name = "nudAnalogTxDeviationCTCSSWideband";
		this.nudAnalogTxDeviationCTCSSWideband.Size = new System.Drawing.Size(47, 20);
		this.nudAnalogTxDeviationCTCSSWideband.TabIndex = 2;
		this.lblAnalogTxDeviationWide.AutoSize = true;
		this.lblAnalogTxDeviationWide.Location = new System.Drawing.Point(183, 13);
		this.lblAnalogTxDeviationWide.Name = "lblAnalogTxDeviationWide";
		this.lblAnalogTxDeviationWide.Size = new System.Drawing.Size(32, 13);
		this.lblAnalogTxDeviationWide.TabIndex = 1;
		this.lblAnalogTxDeviationWide.Text = "Wide";
		this.nudAnalogTxDeviation1750Tone.Location = new System.Drawing.Point(71, 53);
		this.nudAnalogTxDeviation1750Tone.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogTxDeviation1750Tone.Name = "nudAnalogTxDeviation1750Tone";
		this.nudAnalogTxDeviation1750Tone.Size = new System.Drawing.Size(47, 20);
		this.nudAnalogTxDeviation1750Tone.TabIndex = 2;
		this.lblToneBurst.AutoSize = true;
		this.lblToneBurst.Location = new System.Drawing.Point(2, 53);
		this.lblToneBurst.Name = "lblToneBurst";
		this.lblToneBurst.Size = new System.Drawing.Size(58, 13);
		this.lblToneBurst.TabIndex = 1;
		this.lblToneBurst.Text = "Tone burst";
		this.nudAnalogTxDeviationDTMF.Location = new System.Drawing.Point(71, 28);
		this.nudAnalogTxDeviationDTMF.Maximum = new decimal(new int[4] { 255, 0, 0, 0 });
		this.nudAnalogTxDeviationDTMF.Name = "nudAnalogTxDeviationDTMF";
		this.nudAnalogTxDeviationDTMF.Size = new System.Drawing.Size(47, 20);
		this.nudAnalogTxDeviationDTMF.TabIndex = 2;
		this.lblDTMF.AutoSize = true;
		this.lblDTMF.Location = new System.Drawing.Point(25, 32);
		this.lblDTMF.Name = "lblDTMF";
		this.lblDTMF.Size = new System.Drawing.Size(37, 13);
		this.lblDTMF.TabIndex = 1;
		this.lblDTMF.Text = "DTMF";
		this.grpIFGain.Controls.Add(this.lblDigitalGainNarrow);
		this.grpIFGain.Controls.Add(this.lblDigitalGainWide);
		this.grpIFGain.Controls.Add(this.nudDigitalRxGainNarrowband);
		this.grpIFGain.Controls.Add(this.nudDigitalRxGainWideband);
		this.grpIFGain.Controls.Add(this.lblDigitalGainRx);
		this.grpIFGain.Location = new System.Drawing.Point(211, 274);
		this.grpIFGain.Name = "grpIFGain";
		this.grpIFGain.Size = new System.Drawing.Size(215, 77);
		this.grpIFGain.TabIndex = 12;
		this.grpIFGain.TabStop = false;
		this.grpIFGain.Text = "IF gain";
		this.lblDigitalGainNarrow.AutoSize = true;
		this.lblDigitalGainNarrow.Location = new System.Drawing.Point(147, 26);
		this.lblDigitalGainNarrow.Name = "lblDigitalGainNarrow";
		this.lblDigitalGainNarrow.Size = new System.Drawing.Size(27, 13);
		this.lblDigitalGainNarrow.TabIndex = 6;
		this.lblDigitalGainNarrow.Text = "Fine";
		this.lblDigitalGainWide.AutoSize = true;
		this.lblDigitalGainWide.Location = new System.Drawing.Point(60, 27);
		this.lblDigitalGainWide.Name = "lblDigitalGainWide";
		this.lblDigitalGainWide.Size = new System.Drawing.Size(40, 13);
		this.lblDigitalGainWide.TabIndex = 5;
		this.lblDigitalGainWide.Text = "Course";
		this.nudDigitalRxGainNarrowband.Location = new System.Drawing.Point(139, 45);
		this.nudDigitalRxGainNarrowband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudDigitalRxGainNarrowband.Name = "nudDigitalRxGainNarrowband";
		this.nudDigitalRxGainNarrowband.Size = new System.Drawing.Size(65, 20);
		this.nudDigitalRxGainNarrowband.TabIndex = 1;
		this.nudDigitalRxGainWideband.Location = new System.Drawing.Point(52, 45);
		this.nudDigitalRxGainWideband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudDigitalRxGainWideband.Name = "nudDigitalRxGainWideband";
		this.nudDigitalRxGainWideband.Size = new System.Drawing.Size(65, 20);
		this.nudDigitalRxGainWideband.TabIndex = 1;
		this.lblDigitalGainRx.AutoSize = true;
		this.lblDigitalGainRx.Location = new System.Drawing.Point(6, 47);
		this.lblDigitalGainRx.Name = "lblDigitalGainRx";
		this.lblDigitalGainRx.Size = new System.Drawing.Size(20, 13);
		this.lblDigitalGainRx.TabIndex = 0;
		this.lblDigitalGainRx.Text = "Rx";
		this.grpAnalogGain.Controls.Add(this.lblAnalogAudioGainNarrow);
		this.grpAnalogGain.Controls.Add(this.lblAnalogAudioGainWide);
		this.grpAnalogGain.Controls.Add(this.nudAnalogRxGainNarrowband);
		this.grpAnalogGain.Controls.Add(this.nudAnalogRxGainWideband);
		this.grpAnalogGain.Controls.Add(this.lblAnalogAudioGainRx);
		this.grpAnalogGain.Controls.Add(this.lblAnalogMicGain);
		this.grpAnalogGain.Controls.Add(this.nudAnalogMicGain);
		this.grpAnalogGain.Location = new System.Drawing.Point(211, 359);
		this.grpAnalogGain.Name = "grpAnalogGain";
		this.grpAnalogGain.Size = new System.Drawing.Size(215, 96);
		this.grpAnalogGain.TabIndex = 13;
		this.grpAnalogGain.TabStop = false;
		this.grpAnalogGain.Text = "Audio gain";
		this.lblAnalogAudioGainNarrow.AutoSize = true;
		this.lblAnalogAudioGainNarrow.Location = new System.Drawing.Point(138, 22);
		this.lblAnalogAudioGainNarrow.Name = "lblAnalogAudioGainNarrow";
		this.lblAnalogAudioGainNarrow.Size = new System.Drawing.Size(27, 13);
		this.lblAnalogAudioGainNarrow.TabIndex = 10;
		this.lblAnalogAudioGainNarrow.Text = "Fine";
		this.lblAnalogAudioGainWide.AutoSize = true;
		this.lblAnalogAudioGainWide.Location = new System.Drawing.Point(53, 22);
		this.lblAnalogAudioGainWide.Name = "lblAnalogAudioGainWide";
		this.lblAnalogAudioGainWide.Size = new System.Drawing.Size(40, 13);
		this.lblAnalogAudioGainWide.TabIndex = 9;
		this.lblAnalogAudioGainWide.Text = "Course";
		this.nudAnalogRxGainNarrowband.Location = new System.Drawing.Point(130, 40);
		this.nudAnalogRxGainNarrowband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudAnalogRxGainNarrowband.Name = "nudAnalogRxGainNarrowband";
		this.nudAnalogRxGainNarrowband.Size = new System.Drawing.Size(65, 20);
		this.nudAnalogRxGainNarrowband.TabIndex = 7;
		this.nudAnalogRxGainWideband.Location = new System.Drawing.Point(43, 40);
		this.nudAnalogRxGainWideband.Maximum = new decimal(new int[4] { 65535, 0, 0, 0 });
		this.nudAnalogRxGainWideband.Name = "nudAnalogRxGainWideband";
		this.nudAnalogRxGainWideband.Size = new System.Drawing.Size(65, 20);
		this.nudAnalogRxGainWideband.TabIndex = 8;
		this.lblAnalogAudioGainRx.AutoSize = true;
		this.lblAnalogAudioGainRx.Location = new System.Drawing.Point(7, 42);
		this.lblAnalogAudioGainRx.Name = "lblAnalogAudioGainRx";
		this.lblAnalogAudioGainRx.Size = new System.Drawing.Size(20, 13);
		this.lblAnalogAudioGainRx.TabIndex = 1;
		this.lblAnalogAudioGainRx.Text = "Rx";
		this.calibrationTXIandQ.Cols = 8;
		this.calibrationTXIandQ.CtrlText = "DMR Tx 4FSK";
		this.calibrationTXIandQ.Location = new System.Drawing.Point(8, 178);
		this.calibrationTXIandQ.Name = "calibrationTXIandQ";
		this.calibrationTXIandQ.Names = new string[8] { "label_0_0", "label_0_1", "label_0_2", "label_0_3", "label_0_4", "label_0_5", "label_0_6", "label_0_7" };
		this.calibrationTXIandQ.Rows = 1;
		this.calibrationTXIandQ.Size = new System.Drawing.Size(420, 98);
		this.calibrationTXIandQ.TabIndex = 9;
		this.calibrationTXIandQ.Text = "DMR Tx 4FSK";
		this.calibrationTXIandQ.Values = new int[8];
		this.calibrationPowerControlLow.Cols = 16;
		this.calibrationPowerControlLow.CtrlText = "1W (Low power)";
		this.calibrationPowerControlLow.Location = new System.Drawing.Point(7, 95);
		this.calibrationPowerControlLow.Name = "calibrationPowerControlLow";
		this.calibrationPowerControlLow.Names = new string[16]
		{
			"label_0_0", "label_0_1", "label_0_2", "label_0_3", "label_0_4", "label_0_5", "label_0_6", "label_0_7", "label_0_8", "label_0_9",
			"label_0_10", "label_0_11", "label_0_12", "label_0_13", "label_0_14", "label_0_15"
		};
		this.calibrationPowerControlLow.Rows = 1;
		this.calibrationPowerControlLow.Size = new System.Drawing.Size(817, 97);
		this.calibrationPowerControlLow.TabIndex = 9;
		this.calibrationPowerControlLow.Text = "1W (Low power)";
		this.calibrationPowerControlLow.Values = new int[16];
		this.calibrationPowerControlHigh.Cols = 16;
		this.calibrationPowerControlHigh.CtrlText = "5W (High power)";
		this.calibrationPowerControlHigh.Location = new System.Drawing.Point(7, 5);
		this.calibrationPowerControlHigh.Name = "calibrationPowerControlHigh";
		this.calibrationPowerControlHigh.Names = new string[16]
		{
			"label_0_0", "label_0_1", "label_0_2", "label_0_3", "label_0_4", "label_0_5", "label_0_6", "label_0_7", "label_0_8", "label_0_9",
			"label_0_10", "label_0_11", "label_0_12", "label_0_13", "label_0_14", "label_0_15"
		};
		this.calibrationPowerControlHigh.Rows = 1;
		this.calibrationPowerControlHigh.Size = new System.Drawing.Size(817, 97);
		this.calibrationPowerControlHigh.TabIndex = 9;
		this.calibrationPowerControlHigh.Text = "5W (High power)";
		this.calibrationPowerControlHigh.Values = new int[16];
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.grpAnalogGain);
		base.Controls.Add(this.grpIFGain);
		base.Controls.Add(this.grpAnalogTxDeviation);
		base.Controls.Add(this.grpSMeter);
		base.Controls.Add(this.calibrationTXIandQ);
		base.Controls.Add(this.calibrationPowerControlLow);
		base.Controls.Add(this.calibrationPowerControlHigh);
		base.Controls.Add(this.grpSquelch);
		base.Controls.Add(this.nudReceiveAGCTarget);
		base.Controls.Add(this.lblReceiveAGCTarget);
		base.Controls.Add(this.nudVhfOscRef);
		base.Controls.Add(this.lblReferenceOscTuning);
		base.Name = "CalibrationBandControl";
		base.Size = new System.Drawing.Size(844, 511);
		this.grpSquelch.ResumeLayout(false);
		this.grpSquelch.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowTightClose).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideTightClose).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowTightOpen).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideTightOpen).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowNormClose).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideNormClose).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchNarrowNormOpen).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSquelchWideNormOpen).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudVhfOscRef).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudReceiveAGCTarget).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogMicGain).EndInit();
		this.grpSMeter.ResumeLayout(false);
		this.grpSMeter.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudSMeterHigh).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudSMeterLow).EndInit();
		this.grpAnalogTxDeviation.ResumeLayout(false);
		this.grpAnalogTxDeviation.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxGainNarrowband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxGainWideband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationDCSNarrowband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudlAnalogTxDeviationDCSWideband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationCTCSSNarrowband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationCTCSSWideband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviation1750Tone).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogTxDeviationDTMF).EndInit();
		this.grpIFGain.ResumeLayout(false);
		this.grpIFGain.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudDigitalRxGainNarrowband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudDigitalRxGainWideband).EndInit();
		this.grpAnalogGain.ResumeLayout(false);
		this.grpAnalogGain.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogRxGainNarrowband).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nudAnalogRxGainWideband).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}

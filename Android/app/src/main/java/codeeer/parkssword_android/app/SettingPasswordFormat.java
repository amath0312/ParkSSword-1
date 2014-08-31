package codeeer.parkssword_android.app;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.Date;


public class SettingPasswordFormat extends ActionBarActivity {

    Button BT_Save;

    Switch Switch_Manual_CPU;
    Switch Switch_Manual_Hard;
    Switch Switch_Manual_USB;

    SeekBar SB_Manual_Length;
    SeekBar SB_Manual_MixTimes;

    TextView TV_1;
    TextView TV_2;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting_password_format);

        BT_Save = (Button)findViewById(R.id.button_saveformat);

        Switch_Manual_CPU = (Switch)findViewById(R.id.switch_Focus_Manual_CPU);
        Switch_Manual_Hard = (Switch)findViewById(R.id.switch_Focus_Manual_Hard);
        Switch_Manual_USB = (Switch)findViewById(R.id.switch_Focus_Manual_USB);

        SB_Manual_Length = (SeekBar)findViewById(R.id.seekBar_Focus_Manual_PasswordLength);
        SB_Manual_MixTimes = (SeekBar)findViewById(R.id.seekBar_Focus_Manual_MixTimes);

        Switch_Manual_CPU.setOnCheckedChangeListener(CheckedChange);
        Switch_Manual_Hard.setOnCheckedChangeListener(CheckedChange);
        Switch_Manual_USB.setOnCheckedChangeListener(CheckedChange);

        TV_1 = (TextView)findViewById(R.id.textView_Focus_PasswordLength);
        TV_2 = (TextView)findViewById(R.id.textView_Focus_MixTimes);

        SB_Manual_Length.setOnSeekBarChangeListener(SeekBarChange);
        SB_Manual_MixTimes.setOnSeekBarChangeListener(SeekBarChange);

        //配置数据
        Switch_Manual_CPU.setChecked(Functions.Setting_Default.LockCPU);
        Switch_Manual_Hard.setChecked(Functions.Setting_Default.LockHard);
        Switch_Manual_USB.setChecked(Functions.Setting_Default.LockUSB);
        SB_Manual_Length.setProgress(8);//初始化设定
        SB_Manual_MixTimes.setProgress(8);
        SB_Manual_Length.setProgress(Functions.Setting_Default.Length);
        SB_Manual_MixTimes.setProgress(Functions.Setting_Default.MD5Times);

        BT_Save.setEnabled(false);
    }

    CompoundButton.OnCheckedChangeListener CheckedChange = new CompoundButton.OnCheckedChangeListener() {
        @Override
        public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
            BT_Save.setEnabled(true);
        }
    };

    SeekBar.OnSeekBarChangeListener SeekBarChange = new SeekBar.OnSeekBarChangeListener() {
        @Override
        public void onProgressChanged(SeekBar seekBar, int i, boolean b) {
            if(seekBar.getProgress()<8){
                seekBar.setProgress(8);
            }
            TV_1.setText("密码位数 " + SB_Manual_Length.getProgress());
            TV_2.setText("混淆次数 " + SB_Manual_MixTimes.getProgress());
        }

        @Override
        public void onStartTrackingTouch(SeekBar seekBar) {

        }

        @Override
        public void onStopTrackingTouch(SeekBar seekBar) {
            BT_Save.setEnabled(true);
        }
    };

    public void Button_SavePasswordFormat(View view) {
        BT_Save.setEnabled(false);

        SimpleDateFormat df = new SimpleDateFormat("yyyyMMddHHmm");
        String NowTimeStamp = df.format(new Date());

        Functions.Setting_Default.TimeStamp = NowTimeStamp;
        Functions.Setting_Default.Length = SB_Manual_Length.getProgress();
        Functions.Setting_Default.MD5Times = SB_Manual_MixTimes.getProgress();

        Functions.Setting_Default.LockCPU = Switch_Manual_CPU.isChecked();
        Functions.Setting_Default.LockHard = Switch_Manual_Hard.isChecked();
        Functions.Setting_Default.LockUSB = Switch_Manual_USB.isChecked();

        SqliteHelper sh = new SqliteHelper(SettingPasswordFormat.this);
        HandleDB.RefreshPasswordStruct2Sqlite(sh);
    }
}

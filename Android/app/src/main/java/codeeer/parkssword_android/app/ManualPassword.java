package codeeer.parkssword_android.app;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Looper;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.SeekBar;
import android.widget.Switch;
import android.widget.TextView;
import android.widget.Toast;

import java.util.Random;

import codeeer.parkssword_android.app.R;

import static codeeer.parkssword_android.app.Functions.PasswordFormat;
import static codeeer.parkssword_android.app.HandleDB.ManualList.LoadManualItems;

public class ManualPassword extends ActionBarActivity {

    SqliteHelper sh;

    EditText ET;
    CheckBox CB_Number;
    CheckBox CB_AZ_Big;
    CheckBox CB_AZ_Small;
    CheckBox CB_Special_1;
    CheckBox CB_Special_2;
    Button BTCreat;
    Button BTSave;
    Switch S_1;
    Switch S_2;
    Switch S_3;
    SeekBar SB_1;
    SeekBar SB_2;
    TextView TV_1;
    TextView TV_2;

    //密码池
    String PasswordPool="";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_manual_password);

        sh = new SqliteHelper(this);
        ET = (EditText)findViewById(R.id.editText_Manual_Domin);
        CB_Number = (CheckBox)findViewById(R.id.checkBox_Number);
        CB_AZ_Big = (CheckBox)findViewById(R.id.checkBox_Manual_AZ_Big);
        CB_AZ_Small = (CheckBox)findViewById(R.id.checkBox_Manual_AZ_Small);
        CB_Special_1 = (CheckBox)findViewById(R.id.checkBox_Manual_Special_1);
        CB_Special_2 = (CheckBox)findViewById(R.id.checkBox_Manual_Special_2);
        BTCreat = (Button)findViewById(R.id.button_Manual_Create);
        BTSave = (Button)findViewById(R.id.button_Manual_Save);
        S_1 = (Switch)findViewById(R.id.switch_Manual_CPU);
        S_2 = (Switch)findViewById(R.id.switch_Manual_Hard);
        S_3 = (Switch)findViewById(R.id.switch_Manual_USB);
        SB_1 = (SeekBar)findViewById(R.id.seekBar_Manual_PasswordLength);
        SB_2 = (SeekBar)findViewById(R.id.seekBar_Manual_MixTimes);
        TV_1 = (TextView)findViewById(R.id.textView_Manual_Password_Length);
        TV_2 = (TextView)findViewById(R.id.textView_Manual_MixTimes);

        Intent intent = getIntent();
        String domin = intent.getStringExtra("domin");
        ET.setText(domin);

        SB_1.setProgress(8);
        SB_2.setProgress(8);

        //对已经存在于手动列表中的数据进行取值
        Functions.ManualItems data = null;
        data = HandleDB.ManualList.LoadManualItem(sh, domin);
        if(data!=null){
            S_1.setChecked(data.LockCPU);
            S_2.setChecked(data.LockHard);
            S_3.setChecked(data.LockUSB);
            SB_1.setProgress(data.Length);
            SB_2.setProgress(data.MD5Times);

            BTCreat.setTextSize(10);
            BTCreat.setText(data.PWDPool);

            BTSave.setTextSize(10);
            BTSave.setText(Functions.CreatCore(ET.getText().toString()));
        }

        SB_1.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                if(seekBar.getProgress()<8){
                    seekBar.setProgress(8);
                }
                TV_1.setText("密码长度 " + seekBar.getProgress());
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
            }
        });

        SB_2.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                if(seekBar.getProgress()<8){
                    seekBar.setProgress(8);
                }
                TV_2.setText("混淆次数 " + seekBar.getProgress());
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {

            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
            }
        });

        CB_Number.setOnCheckedChangeListener(new CheckChange());
        CB_AZ_Big.setOnCheckedChangeListener(new CheckChange());
        CB_AZ_Small.setOnCheckedChangeListener(new CheckChange());
        CB_Special_1.setOnCheckedChangeListener(new CheckChange());
        CB_Special_2.setOnCheckedChangeListener(new CheckChange());

        BTCreat.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                if(PasswordPool.equals("")) {
                    String BaseStr = "";
                    if (CB_Number.isChecked()) BaseStr += "0123456789";
                    if (CB_AZ_Big.isChecked()) BaseStr += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    if (CB_AZ_Small.isChecked()) BaseStr += "abcdefghijklmnopqrstuvwxyz";
                    if (CB_Special_1.isChecked()) BaseStr += "!@#,.?";
                    if (CB_Special_2.isChecked()) BaseStr += "-=_+";
                    PasswordPool = BaseStr;
                    BTCreat.setTextSize(10);
                }
                PasswordPool = CreatPWDPool(PasswordPool);
                BTCreat.setText(PasswordPool);

                return false;
            }
        });
    }


    public class CheckChange implements CompoundButton.OnCheckedChangeListener{
        @Override
        public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
            PasswordPool = "";
            BTCreat.setEnabled(CB_Number.isChecked()||CB_AZ_Big.isChecked()||CB_AZ_Small.isChecked()||CB_Special_1.isChecked()||CB_Special_2.isChecked());
        }
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        return super.onOptionsItemSelected(item);
    }

    //生成密码池
    public String CreatPWDPool(String BaseStr){
        for(int i=0; i<10; i++){BaseStr=RandTheString(BaseStr);}
        return BaseStr;
    }

    String RandTheString(String Source)
    {
        //随机打乱置后
        String Temp = Source;
        Random RD = new Random();
        int TempNum = RD.nextInt(Temp.length());
        String TempChar = String.valueOf(Temp.charAt(TempNum));

        StringBuffer SB = new StringBuffer(Temp);
        SB = SB.deleteCharAt(TempNum);
        Temp = SB.toString();

        Temp += TempChar;

        //半数打乱
        if (Temp.length() > 2)
        {
            String T1 = Temp.substring(2, Temp.length() / 2);
            Temp = Temp.replace(T1, "");
            Temp = T1 + Temp;
        }

        //头置尾
        String C1 = String.valueOf(Temp.charAt(0));
        Temp = Temp.substring(1, Temp.length()) + C1;

        return Temp;
    }

    public void Button_Manual_Creat(View view) {
        BTSave.setTextSize(10);
        BTSave.setText(Functions.CreatCore(ET.getText().toString(),PasswordPool,S_1.isChecked(),S_2.isChecked(),S_3.isChecked(),SB_1.getProgress(),SB_2.getProgress()));
        BTSave.setEnabled(true);
    }

    public void Button_Manual_Save(View view) {
        HandleDB.ManualList.InsertOrUpdateManualItem(sh, ET.getText().toString(), PasswordPool, SB_1.getProgress(), SB_2.getProgress(), S_1.isChecked(), S_2.isChecked(), S_3.isChecked());
        new AlertDialog.Builder(this)
                .setIcon(R.drawable.abc_ic_go)
                .setMessage("密码已经保存")
                .setPositiveButton("确定", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int whichButton) {
                        finish();
                    }
                }).show();
    }
}

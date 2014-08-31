package codeeer.parkssword_android.app;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;


public class SettingHardInfo extends ActionBarActivity {

    EditText ET_CPU;
    EditText ET_Hard;
    EditText ET_USB;
    Button BT_Save;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting_hard_info);
        ET_CPU = (EditText)findViewById(R.id.editText_CPUID);
        ET_Hard = (EditText)findViewById(R.id.editText_HardID);
        ET_USB = (EditText)findViewById(R.id.editText_USBID);
        BT_Save = (Button)findViewById(R.id.button_SaveHardInfo);

        ET_CPU.addTextChangedListener(Watcher);
        ET_Hard.addTextChangedListener(Watcher);
        ET_USB.addTextChangedListener(Watcher);

        ET_CPU.setText(Functions.CPUCodeStr);
        ET_Hard.setText(Functions.HardCodeStr);
        ET_USB.setText(Functions.USBCodeStr);
        BT_Save.setEnabled(false);
    }

    TextWatcher Watcher=new TextWatcher() {
        @Override
        public void beforeTextChanged(CharSequence charSequence, int i, int i2, int i3) {

        }

        @Override
        public void onTextChanged(CharSequence charSequence, int i, int i2, int i3) {
            BT_Save.setEnabled(true);
        }

        @Override
        public void afterTextChanged(Editable editable) {

        }
    };

    public void Button_SaveHardInfo(View view) {
        BT_Save.setEnabled(false);
        Functions.CPUCodeStr=ET_CPU.getText().toString();
        Functions.HardCodeStr=ET_Hard.getText().toString();
        Functions.USBCodeStr=ET_USB.getText().toString();

        SqliteHelper sh = new SqliteHelper(SettingHardInfo.this);
        HandleDB.RefreshHardInfo2Sqlite(sh);
    }
}

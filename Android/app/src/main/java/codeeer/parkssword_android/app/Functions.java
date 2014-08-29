package codeeer.parkssword_android.app;

import android.app.AlertDialog;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.DialogInterface;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import static codeeer.parkssword_android.app.HandleDB.ManualList.LoadManualItems;

public class Functions{

    //设定（默认）
    public static class Setting_Default
    {
        public static int Length = 8;
        public static int MD5Times = 8;
        public static boolean LockCPU = true;
        public static boolean LockHard = false;
        public static boolean LockUSB = false;

        public static String TimeStamp = "";//时间戳
        public static String LoginEmail = "";//登录Email

        public static String CloudyTime = "";//云同步时间
        public static int UpdateSinceLstCloudy = 0;//记录自从上次云同步以来更新的数据条目
    }

    //CPU编码
    public static String CPUCodeStr = null;

    //硬盘编码
    public static String HardCodeStr = null;

    //USB编码
    public static String USBCodeStr = null;

    //手动密码结构
    public static class ManualItems
    {
        public ManualItems(String Domin,String TimeStamp,String PWDPool,int Length,int MD5Times,boolean LockCPU,boolean LockHard,boolean LockUSB){
            this.Domin=Domin;
            this.TimeStamp=TimeStamp;
            this.PWDPool=PWDPool;
            this.Length=Length;
            this.MD5Times=MD5Times;
            this.LockCPU=LockCPU;
            this.LockHard=LockHard;
            this.LockUSB=LockUSB;
        }

        //域名
        public String Domin;
        //时间戳
        public String TimeStamp;
        //密码池
        public String PWDPool;
        //密码长度
        public int Length;
        //MD5计算次数
        public int MD5Times;
        //绑定CPU
        public boolean LockCPU;
        //绑定Hard Disk
        public boolean LockHard;
        //绑定USB
        public boolean LockUSB;
    }

    //用以保存手动密码详细信息
    public static List<ManualItems> ManualItemsLst = new ArrayList<ManualItems>();

    //从数据库加载基础数据
    public static void LoadBaseInfo(SqliteHelper sh){

        //加载基础数据
        CPUCodeStr=HandleDB.findValueByKey(sh,"CPUID");
        HardCodeStr=HandleDB.findValueByKey(sh,"HardID");
        USBCodeStr=HandleDB.findValueByKey(sh,"USBID");

        Setting_Default.TimeStamp = HandleDB.findValueByKey(sh,"TimeStamp");
        Setting_Default.Length = Integer.parseInt(HandleDB.findValueByKey(sh,"Length_Default"));
        Setting_Default.MD5Times = Integer.parseInt(HandleDB.findValueByKey(sh,"MD5Times_Default"));
        Setting_Default.LockCPU = Boolean.parseBoolean(HandleDB.findValueByKey(sh, "LockCPU_Default"));
        Setting_Default.LockHard = Boolean.parseBoolean(HandleDB.findValueByKey(sh, "LockHard_Default"));
        Setting_Default.LockUSB = Boolean.parseBoolean(HandleDB.findValueByKey(sh, "LockUSB_Default"));

        MainActivity.DominLst = HandleDB.ManualList.ReadEnumDomin(sh);

        //加载手动列表
        ManualItemsLst.clear();
        ManualItemsLst = LoadManualItems(sh);
    }

    static int IsManualPWD(String Source)
    {
        boolean Has = false;
        int Index = -1;

        if(ManualItemsLst!=null) {
            for (int i = 0; i < ManualItemsLst.size(); i++) {
                ManualItems Item = ManualItemsLst.get(i);
                if (Item.Domin.toLowerCase().equals(Source.toLowerCase())) {
                    Has = true;
                    Index = i;
                    break;
                }
            }
        }
        return Has ? Index : -1;
    }

    public static String CreatCore(String SourceText){
        try
        {
            String TempStr = SourceText;

            for (int i = 0; i < 16; i++)
            {
                TempStr = MD5.Bit16(TempStr).toUpperCase();
            }

            int Find = IsManualPWD(SourceText);

            if (Find != -1)
            {
                ManualItems Item = ManualItemsLst.get(Find);

                //启用手动加密序列
                String CPUID = Item.LockCPU ? CPUCodeStr : "";
                String HardID = Item.LockHard ? HardCodeStr : "";
                String USBID = Item.LockUSB ? USBCodeStr : "";

                TempStr = MD5.Manual(TempStr, Item.PWDPool, Item.Length, Item.MD5Times, CPUID, HardID, USBID);
            }
            else
            {
                //默认模式计算密码
                //混合参数箱子
                StringBuilder MixStr = new StringBuilder(TempStr);

                if (Setting_Default.LockCPU) MixStr.append(CPUCodeStr);
                if (Setting_Default.LockHard) MixStr.append(HardCodeStr);
                if (Setting_Default.LockUSB) MixStr.append(USBCodeStr);

                String Temp01 = MD5.Bit32(MixStr.toString()).substring(0, Setting_Default.Length).toUpperCase();
                String Temp02 = MD5.Bit32(Temp01 + MixStr.toString()).substring(0, Setting_Default.Length).toLowerCase();

                TempStr = Temp01 + ":" + Temp02;
            }

            return TempStr;
        }
        catch (Exception E)
        {
            throw new RuntimeException(E.getMessage());
        }
    }

    public static String CreatCore(String SourceText,String _PasswordPool,boolean _LockCPU,boolean _LockHard,boolean _LockUSB,int _PasswordLength,int _MD5Times){
        try
        {
            String TempStr = SourceText;

            for (int i = 0; i < 16; i++)
            {
                TempStr = MD5.Bit16(TempStr).toUpperCase();
            }

            //启用手动加密序列
            String CPUID = _LockCPU ? CPUCodeStr : "";
            String HardID = _LockHard ? HardCodeStr : "";
            String USBID = _LockUSB ? USBCodeStr : "";

            TempStr = MD5.Manual(TempStr, _PasswordPool, _PasswordLength, _MD5Times, CPUID, HardID, USBID);

            return TempStr;
        }
        catch (Exception E)
        {
            throw new RuntimeException(E.getMessage());
        }
    }

    public static String PasswordFormat(String SourcePassword) throws Exception {

        String Temp = SourcePassword;

        for (int i = 0; i < 20; i++)
        {
            Temp = MD5.Bit32(Temp);
        }

        return Temp;
    }
}


class HtmlRequest {

    public static String PostData(URL url,String params) {

        byte[] data = params.getBytes();//获得请求体
        try {
            HttpURLConnection httpURLConnection = (HttpURLConnection)url.openConnection();
            httpURLConnection.setConnectTimeout(5000);              //设置连接超时时间
            httpURLConnection.setDoInput(true);                     //打开输入流，以便从服务器获取数据
            httpURLConnection.setDoOutput(true);                    //打开输出流，以便向服务器提交数据
            httpURLConnection.setRequestMethod("POST");             //设置以Post方式提交数据
            httpURLConnection.setUseCaches(false);                  //使用Post方式不能使用缓存
            //设置请求体的类型是文本类型 application/octet-stream
            httpURLConnection.setRequestProperty("Content-Type", "application/octet-stream");
            //设置请求体的长度
            httpURLConnection.setRequestProperty("Content-Length", String.valueOf(data.length));
            //获得输出流，向服务器写入数据
            OutputStream outputStream = httpURLConnection.getOutputStream();
            outputStream.write(data);

            int response = httpURLConnection.getResponseCode();            //获得服务器的响应码
            if(response == HttpURLConnection.HTTP_OK) {
                InputStream inptStream = httpURLConnection.getInputStream();
                return dealResponseResult(inptStream);                     //处理服务器的响应结果
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        return "";
    }

    public static String dealResponseResult(InputStream inputStream) {
        String resultData = null;      //存储处理结果
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        byte[] data = new byte[1024];
        int len = 0;
        try {
            while((len = inputStream.read(data)) != -1) {
                byteArrayOutputStream.write(data, 0, len);
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        resultData = new String(byteArrayOutputStream.toByteArray());
        return resultData;
    }

    public static String getURLSource(URL url) throws Exception    {
        HttpURLConnection conn = (HttpURLConnection)url.openConnection();
        conn.setRequestMethod("GET");

        conn.setConnectTimeout(5 * 1000);
        InputStream inStream =  conn.getInputStream();  //通过输入流获取html二进制数据
        byte[] data = readInputStream(inStream);        //把二进制数据转化为byte字节数据
        return new String(data);
    }

    public static byte[] readInputStream(InputStream instream) throws Exception {
        ByteArrayOutputStream outStream = new ByteArrayOutputStream();
        byte[]  buffer = new byte[1204];
        int len;
        while ((len = instream.read(buffer)) != -1){
            outStream.write(buffer,0,len);
        }
        instream.close();
        return outStream.toByteArray();
    }

    //把从服务端获取的数据处理填充到数据库中
    public static String Source2Sqlite(String Source, SqliteHelper sh){

        SQLiteDatabase db = sh.getWritableDatabase();
        db.execSQL("UPDATE Setting SET Value = '"+MainActivity.UserName+"' WHERE Key = 'UserName'");
        db.execSQL("UPDATE Setting SET Value = '"+MainActivity.PassWord+"' WHERE Key = 'PassWord'");

        if(Source.contains("SUCC!")){
            Pattern pattern_Unit = Pattern.compile("#Unit#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Unit#");
            Matcher matcher_Unit = pattern_Unit.matcher(Source);

            Pattern pattern_Setting = Pattern.compile("#Setting#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Setting#");
            Matcher matcher_Setting = pattern_Setting.matcher(Source);

            boolean dropDateBase = false;

            while (matcher_Setting.find()) {
                db.execSQL("UPDATE Setting SET Value = '"+matcher_Setting.group(1)+"' WHERE Key = 'TimeStamp'");
                db.execSQL("UPDATE Setting SET Value = '"+Integer.parseInt(matcher_Setting.group(2))+"' WHERE Key = 'Length_Default'");
                db.execSQL("UPDATE Setting SET Value = '"+Integer.parseInt(matcher_Setting.group(3))+"' WHERE Key = 'MD5Times_Default'");
                db.execSQL("UPDATE Setting SET Value = '"+Boolean.parseBoolean(matcher_Setting.group(4))+"' WHERE Key = 'LockCPU_Default'");
                db.execSQL("UPDATE Setting SET Value = '"+Boolean.parseBoolean(matcher_Setting.group(5))+"' WHERE Key = 'LockHard_Default'");
                db.execSQL("UPDATE Setting SET Value = '"+Boolean.parseBoolean(matcher_Setting.group(6))+"' WHERE Key = 'LockUSB_Default'");
            }

            while (matcher_Unit.find()) {
                if(!dropDateBase){
                    db.execSQL("DELETE FROM ManualPWDs");
                    dropDateBase = true;
                }

                db.execSQL("INSERT INTO ManualPWDs(Domin,TimeStamp,PWDPool,Length,MD5Times,LockCPU,LockHard,LockUSB) VALUES(?,?,?,?,?,?,?,?)",
                        new Object[]{matcher_Unit.group(1),matcher_Unit.group(2),matcher_Unit.group(3), Integer.parseInt(matcher_Unit.group(4)),Integer.parseInt(matcher_Unit.group(5)), Boolean.parseBoolean(matcher_Unit.group(6)),Boolean.parseBoolean(matcher_Unit.group(7)),Boolean.parseBoolean(matcher_Unit.group(8))});
            }

            //从数据库加载基础数据
            Functions.LoadBaseInfo(sh);

            return "数据同步成功";
        }
        else if(Source.indexOf("Wrong UserName or Password!")==0){
            MainActivity.PassWord=SqliteHelper.LogoutPassword;
            return "用户名或密码错误";
        }
        else{
            Log.e("Source From Server",Source);
            return "数据下载失败（"+Source.length()+"）";
        }
    }
}
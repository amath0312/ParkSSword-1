<?php
require_once('Core.php');

//用户信息
$Email=CleanUp($_GET[Email]);
$PassWord=CleanUp($_GET[PassWord]);

//密码加盐
$PassWord=MD5Saltly($PassWord);

//鉴定绑定信息
$Binding_Type=CleanUp($_GET[Binding_Type]);
$Binding_Data=CleanUp($_GET[Binding_Data]);

IsLimitedOrDie();

//生成用于验证的随机数
$RandNum=GetGuid();
$RandNum_Short=substr(GetGuid(),0,4);

if($Email!="" && $PassWord!="" && 
    ($Binding_Data!="" || $Binding_Type=="Weixin")//当Binding_Type为微信时，Binding_Data可以为空
    )
{
    Write2Log("$Email","请求绑定:[$Binding_Type]","SendVerify");

    switch ($Binding_Type)
    {
        case "Email":
            if(WriteToSql($RandNum))
        	{
        		echo V_Email($Email,$Binding_Data,$RandNum);
        	}
        	else
        	{
        		echo "0";
        	}
        break;  

        case "Weixin":
            if(WriteToSql($RandNum_Short))
            {
                echo $RandNum_Short;
            }
            else
            {
                echo "0";
            }
        break;
    }
}

function WriteToSql($Verify_Number)
{	
    global $Binding_Type,$Binding_Data,$Email,$PassWord;
    $SUCC=false;

    //时间戳
    $Time=strtotime(now);

    //写入数据库
    $mysql = new SaeMysql();
    $result = $mysql->getData("SELECT * FROM PSS_User where Email='$Email'");

    if(!empty($result))
    {
        foreach ($result as $row)
        {
            //验证密码
            if($row['PassWord']==$PassWord)
            {
                //验证是否激活账号
                if($row['SignUp']=='0' && $Binding_Type!="Email")
                {
                    echo "Not SignUp!";
                }
                //验证是否存在已经激活的同名帐号
                else if(IsExistEmail($row['Binding_Data']) && $row['Binding_Data']!=$row['Email'])
                {
                    echo "Exist Email!";
                }
                else
                {
                    $mysql->runSql("UPDATE PSS_User SET Active='0',Binding_RequireTime='$Time',Binding_Type='$Binding_Type',Binding_Data='$Binding_Data',Binding_Number='$Verify_Number' WHERE Email = '$Email'");
                    $SUCC=true;
                }
            }
            else
            {
                echo "Wrong Password!";
            }
        }
    }

    return $SUCC;
}
?>
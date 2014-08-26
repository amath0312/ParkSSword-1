<?php
require_once('Core.php');

//识别用户信息
$Email=CleanUp($_GET[Email]);
$PassWord=CleanUp($_GET[PassWord]);

//硬件信息
$CPUID=CleanUp($_GET[CPUID]);
$HardID=CleanUp($_GET[HardID]);
$USBID=CleanUp($_GET[USBID]);

//手动密码单
$XMLData=base64_encode($GLOBALS['HTTP_RAW_POST_DATA']);

$mysql = new SaeMysql();

//验证密码
if($Email!="" && $PassWord!="" && $CPUID!="" && $HardID!="")
{
	//密码加盐
	$PassWord=MD5Saltly($PassWord);

	if(CheckEmailAndPassWord($Email,$PassWord))
	{
		$mysql->runSql("UPDATE PSS_User SET CPU_ID = '$CPUID',Hard_ID ='$HardID' WHERE Email='$Email' and PassWord='$PassWord'");

		if($USBID!="")$mysql->runSql("UPDATE PSS_User SET USB_ID ='$USBID' WHERE Email='$Email' and PassWord='$PassWord'");

		if($XMLData!="")
		{
			$mysql->runSql("UPDATE PSS_User SET XMLData ='$XMLData' WHERE Email='$Email' and PassWord='$PassWord'");//暂停使用，用于测试
			echo $XMLData;
		}

		Write2Log("$Email","上传PC端密码生成策略");
		echo "SUCC!";
	}
	else
	{
		echo "Wrong PassWord!";
	}
}
else
{
	//缺少参数
	echo "Missing Paras!";
}
?>
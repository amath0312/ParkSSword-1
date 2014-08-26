<?php
require_once('Core.php');

$Email=CleanUp($_GET[Email]);
$CPUID=CleanUp($_GET[CPUID]);

$mysql = new SaeMysql();

$Exist=false;
$HasSignUp="0";
$HasWeixin="0";

$result = $mysql->getData("SELECT SignUp,Weixin FROM PSS_User where Email='$Email' and LastLoginInfo='$CPUID'");


if(!empty($result))
{
	foreach ($result as $row)
	{
		$Exist=true;
		$HasSignUp=$row['SignUp'];
		if(trim(trim($row['Weixin'],"　"))!="")$HasWeixin="1";
	}
}

Write2Log("$Email".":"."$CPUID","帐号状态信息查询:$HasSignUp$HasWeixin");

if($Exist)
{
	echo $HasSignUp.$HasWeixin;
}
else
{
	echo "-1";
}
?>
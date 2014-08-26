<?php
require_once('Core.php');

//用户信息
$Email=CleanUp($_GET[Email]);
$PassWord=CleanUp($_GET[PassWord]);
$Type=CleanUp($_GET[Type]);

//密码加盐
$PassWord=MD5Saltly($PassWord);

$mysql = new SaeMysql();

if($Type=="Weixin")
{
	$result = $mysql->getData("SELECT * FROM PSS_User where Email='$Email' and PassWord='$PassWord'");

	$Find=false;

    if(!empty($result)){$Find=true;}

    if($Find)
    {
    	$Find=false;
    	$mysql->runSql("UPDATE PSS_User SET $Type=' ' WHERE Email = '$Email'");
    	Write2Log("$Email","微信解绑成功");
    	echo "SUCC!";
    }
    else
    {
    	Write2Log("$Email","微信解绑失败");
    	echo "Wrong Email or PWD!";
    }
}
else
{
	Write2Log("[ROOT]","入侵拦截:[unbundling.php]错误参数信息");
	echo "Wrong Para!";
}
?>
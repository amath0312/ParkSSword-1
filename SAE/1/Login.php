<?php
require_once('Core.php');

//用户信息
$Email=CleanUp($_GET[Email]);
$PassWord=CleanUp($_GET[PassWord]);
$CPUID=CleanUp($_GET[CPUID]);

//密码加盐
$PassWord=MD5Saltly($PassWord);

$Find=false;

$mysql = new SaeMysql();
$result = $mysql->getData("SELECT * FROM PSS_User WHERE (Email='$Email' AND PassWord='$PassWord')");

if(!empty($result))
{
    foreach ($result as $row)
    {
	    $Find=true;
	    echo "SUCC!";

	    echo "<br>[SignUp=".$row['SignUp']."]";
	    echo "<br>[Weixin=".$row['Weixin']."]";

	    //记录登录信息
	    $mysql->runSql("UPDATE PSS_User SET LastLoginInfo='$CPUID' where (Email='$Email' AND PassWord='$PassWord')");

	    Write2Log("$Email","用户登录成功[CPUDID:".$CPUID."]");
    }
}

if(!$Find)
{
	Write2Log("$Email","用户登录失败");
	echo "Wrong UserName or Password!";
}
?>
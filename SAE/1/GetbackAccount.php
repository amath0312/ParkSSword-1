<?php
require('Core.php');

$Email=CleanUp($_GET[Email]);

if(!preg_match("/^[_.0-9a-z-]+@([0-9a-z][0-9a-z-]+.)+[a-z]{2,3}$/",$Email))
{   
    die("NOT Email!");
} 

//生成用于验证的随机数
$RandNum=GetGuid();

IsLimitedOrDie();

if(IsExistEmail($Email))
{
	Write2DBAndSendEmail4Back($Email,$RandNum);
	Write2Log("$Email","请求找回密码操作","GetbackAccount");
}
else
{
	echo "Not SignUp!";
}

function HasInGetbackList($Email)
{
	$Has=false;
	$mysql = new SaeMysql();
	$result = $mysql->getData("SELECT * FROM GetbackList where Email='$Email'");
	if(!empty($result))$Has=true;
	return $Has;
}

function Write2DBAndSendEmail4Back($Email,$RandNum)
{
	//时间戳
    $Time=strtotime(now);
    $mysql = new SaeMysql();

	//Write2DB
	if(HasInGetbackList($Email))
	{
		if(!$mysql->runSql("UPDATE GetbackList SET TimeStamp='$Time',Vcode='$RandNum' WHERE Email='$Email'"))
	    {
	    	die("Failed!".$mysql->errmsg());
	    }
	}
	else
	{
		if(!$mysql->runSql("INSERT INTO `GetbackList`(`TimeStamp`,`Email`,`Vcode`) VALUES ('$Time','$Email','$RandNum')"))
	    {
	    	die("Failed!".$mysql->errmsg());
	    }
	}

	//SendEmail4Back
    $Body=<<< EOT
<!--HTML-->
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>找回密码</title>
<style type="text/css">
body,td,th {
	font-family: Verdana, Geneva, sans-serif;
}
</style>
</head>
<body>
EOT;
$Body.="<p><a href=\"http://pss.codeeer.com/ChangePWD.php?Email=$Email&Vcode=$RandNum\">点击链接找回密码</a>（链接仅24小时有效）</p><p>或手动输入以下网址：http://pss.codeeer.com/ChangePWD.php?Email=$Email&Vcode=$RandNum</p>";
$Body.=<<< EOT
<h5>(C)2013 ParkSSword 开发团队</h5>
</body>
</html>   
EOT;

	$sc= apibus::init('sendcloud');
	$sc->send_mail(
	  'postmaster@codeeer.sendcloud.org' , //api_user, 需要登录SendCloud创建收信域名获取发送账号
	  'uHjeuvUkheMcLdvL',  // api_key，需要登录SendCloud创建收信域名获取发送账号。
	  'postmaster@codeeer.sendcloud.org', // from， 发件人地址
	  'codeeer 软件中心',  // fromname， 发件人称呼
	  '找回密码',   // subject，邮件主题
	  $Email, //to， 收件人
	  $Body//html， html形式的邮件正文
	);

	echo "SUCC!";
}
?>
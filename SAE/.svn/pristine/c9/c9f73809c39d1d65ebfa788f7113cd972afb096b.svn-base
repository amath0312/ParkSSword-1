<?php
require('Core.php');

$PWD=CleanUp($_GET[PWD]);

if($PWD!="zmkm")die("No Permission!");

$mysql = new SaeMysql();
$result = $mysql->getData("SELECT * FROM LOG order by ID desc limit 0,100");

echo <<< EOF
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<style type="text/css">
#customers
  {
  font-family:"Trebuchet MS", Arial, Helvetica, sans-serif;
  width:100%;
  border-collapse:collapse;
  }

#customers td, #customers th 
  {
  font-size:1em;
  border:1px solid #98bf21;
  padding:3px 7px 2px 7px;
  }

#customers th 
  {
  font-size:1.1em;
  text-align:left;
  padding-top:5px;
  padding-bottom:4px;
  background-color:#3399FF;
  color:#ffffff;
  }

#customers tr.alt td 
  {
  color:#000000;
  background-color:#EAF2D3;
  }
</style>
</head>

<body>
<table id="customers">
<tr>
<th>Attribute</th>
<th>Value</th>
</tr>
EOF;

//输入数据库属性
echo "<tr><td>GetbackList Length</td><td>".$mysql->getLine("select count(*) from GetbackList")."</td></tr>";
echo "<tr><td>IPLimited Length</td><td>".$mysql->getLine("select count(*) from IPLimited")."</td></tr>";
echo "<tr><td>LOG Length</td><td>".$mysql->getLine("select count(*) from LOG")."</td></tr>";
echo "<tr><td>PSS_User Length</td><td>".$mysql->getLine("select count(*) from PSS_User")."</td></tr>";
echo "<tr><td>WeChatRequire Length</td><td>".$mysql->getLine("select count(*) from WeChatRequire")."</td></tr>";

echo <<< EOF
</table>
<br>
EOF;

echo <<< EOF
<table id="customers">
<tr>
<th>ID</th>
<th>Time</th>
<th>IP</th>
<th>From</th>
<th>Event</th>
</tr>
EOF;

//输入日志
if(!empty($result))
{
    foreach ($result as $row)
    {
        echo "<tr><td>".$row['ID']."</td><td>".$row['Time']."</td><td>".$row['IPInfo']."</td><td>".$row['Email']."</td><td>".$row['Event']."</td></tr>";
    }
}

echo <<< EOF
</table>
</body>
</html>
EOF;

?>
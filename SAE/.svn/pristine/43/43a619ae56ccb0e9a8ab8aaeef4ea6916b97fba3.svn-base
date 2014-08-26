<?php
require_once('Core.php');

$Time = strtotime(now);

$mysql = new SaeMysql();

$mysql->runSql("DELETE FROM GetbackList WHERE $Time-TimeStamp>86400");
$mysql->runSql("DELETE FROM WeChatRequire WHERE $Time-TimeStamp>86400");
$mysql->runSql("DELETE FROM IPLimited WHERE $Time-Time>7200");
$mysql->runSql("DELETE FROM PSS_User WHERE $Time-Binding_RequireTime>86400 and SignUp='0'");

Write2Log("[ROOT]","数据表例行清理");

echo $Time;
?>
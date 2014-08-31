<?php
require_once('Core.php');

//识别用户信息
$Email=CleanUp($_GET[Email]);
$PassWord=CleanUp($_GET[PassWord]);
$DeviceID=CleanUp($_GET[DeviceID]);

//手动密码单
$StrData=$GLOBALS['HTTP_RAW_POST_DATA'];

$mysql = new SaeMysql();

echo "#Head#";//头尾标识

//验证密码
if(isset($Email) && isset($PassWord))
{
	//密码加盐
	$PassWord=MD5Saltly($PassWord);

	if(CheckEmailAndPassWord($Email,$PassWord))
	{
		$mysql->runSql("UPDATE PSS_User SET LastLoginDeviceID = '$DeviceID' WHERE Email='$Email' and PassWord='$PassWord'");

		if(isset($StrData))
		{
			$DataFormat='/#Unit#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Unit#/';
			$DataFormat_Setting='/#Setting#@(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)#@#(.*?)@#Setting#/';

			//加载移动终端数据
			preg_match_all($DataFormat, $StrData, $AndroidData);
			preg_match_all($DataFormat_Setting, $StrData, $AndroidData_Setting);

			//加载服务器StrData数据
			$StrDataFromServer=$mysql->getData("SELECT StrData FROM PSS_User WHERE Email='$Email' AND PassWord='$PassWord'");
			$StrDataFromServer=$StrDataFromServer[0]["StrData"];
			if($StrDataFromServer!=""){$StrDataFromServer=base64_decode($StrDataFromServer);}

			preg_match_all($DataFormat, $StrDataFromServer, $ServerData);
			preg_match_all($DataFormat_Setting, $StrDataFromServer, $ServerData_Setting);

			if(count($AndroidData[1])==0)
			{
				//only pull
				echo "Only Pull!";				
				echo $StrDataFromServer;
			}
			else if(count($ServerData[1])==0)
			{
				//only push
				echo "Only Push!";
				echo $StrData;
				$StrData=base64_encode($StrData);
				$mysql->runSql("UPDATE PSS_User SET StrData='$StrData' where Email='$Email'");
			}
			else
			{
				//Change Items
				echo "Change Items!";

				//比对Unit
				for($Android_Index=0;$Android_Index<count($AndroidData[1]);$Android_Index++)
				{
					for($Server_Index=0;$Server_Index<count($ServerData[1]);$Server_Index++)
					{
						if($AndroidData[1][$Android_Index]==$ServerData[1][$Server_Index])
						{
							$AndroidData[2][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[2][$Android_Index]:$ServerData[2][$Server_Index];
							$AndroidData[3][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[3][$Android_Index]:$ServerData[3][$Server_Index];
							$AndroidData[4][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[4][$Android_Index]:$ServerData[4][$Server_Index];
							$AndroidData[5][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[5][$Android_Index]:$ServerData[5][$Server_Index];
							$AndroidData[6][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[6][$Android_Index]:$ServerData[6][$Server_Index];
							$AndroidData[7][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[7][$Android_Index]:$ServerData[7][$Server_Index];
							$AndroidData[8][$Android_Index]=$AndroidData[2][$Android_Index]>$ServerData[2][$Server_Index]?$AndroidData[8][$Android_Index]:$ServerData[8][$Server_Index];
						}
					}
				}

				//比对Setting
				$NewSetting = "";

				if($AndroidData_Setting[1][0]>$ServerData_Setting[1][0])
				{
					$NewSetting=$AndroidData_Setting[0][0];
				}
				else
				{
					$NewSetting=$ServerData_Setting[0][0];
				}

				//合并生成新数组
				$NewData = array_merge($AndroidData,$ServerData);
				$NewData2Str = "";
				//储存并输出
				for($Index=0;$Index<count($NewData[1]);$Index++)
				{
					$TempStr="#Unit#@".$NewData[1][$Index]."#@#".$NewData[2][$Index]."#@#".$NewData[3][$Index]."#@#".$NewData[4][$Index]."#@#".$NewData[5][$Index]."#@#".$NewData[6][$Index]."#@#".$NewData[7][$Index]."#@#".$NewData[8][$Index]."@#Unit#";
					$NewData2Str.=$TempStr;
				}
				$FinalData=$NewSetting.$NewData2Str;
				echo $FinalData;
				$FinalData=base64_encode($FinalData);
				$mysql->runSql("UPDATE PSS_User SET StrData='$FinalData' where Email='$Email'");
			}
			
			Write2Log("$Email","上传Android端密码生成策略");
			echo "SUCC!";
		}
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

echo "#Tail#";//头尾标识
?>
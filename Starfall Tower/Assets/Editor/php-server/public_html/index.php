<?php
error_reporting(0);
ini_set('display_errors', 'Off');
ini_set('display_startup_errors', 'Off');

function redirect($url, $permanent = false)
{
    if (headers_sent() === false)
    {
        header('Location: ' . $url, true, ($permanent === true) ? 301 : 302);
    }
	else
	{
		echo "<script type=\"text/javascript\">
           window.location = \"$url\"
		</script>";
	}

    exit();
}

function isValidJSON($str) {
   json_decode($str);
   return json_last_error() == JSON_ERROR_NONE;
}

function isNullOrEmptyString($str){
    return ($str === null || trim($str) === '');
}

function logInFile($str_log) {
	$path = 'logs';
	
	if (!file_exists($path)) {
		mkdir($path, 0777, true);
	}
	
	$date = new DateTime();
	
    file_put_contents($path.'/'.$date->format("Ymd").'_logs.txt', '['.$date->format("Y-m-d H:i:s").'] '.$str_log.PHP_EOL, FILE_APPEND | LOCK_EX);
};

function returnJsonData($data) {
	$json = json_encode($data);
	header("Content-Type: application/json");
	echo $json;
	exit();
}

function returnJsonMsg($result, $msg = null) {
	$data = [
		'result' => $result
	];
	if (!isNullOrEmptyString($msg)) {
		$data['message'] = $msg;
	}
	returnJsonData($data);
}

$json_file_path = "unity_archives.json";
$json_object = file_get_contents($json_file_path);
$json_data = json_decode($json_object, true);

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
	$postData = file_get_contents('php://input');
	if (strlen($postData) > 0 && isValidJSON($postData)) {
		$data = json_decode($postData, true);
	
		if (isset($data['version']) && isset($data['link'])) {
			$ver = $data['version'];
			$link = $data['link'];
			
			if (!isNullOrEmptyString($ver) && !isNullOrEmptyString($link)) {
				$json_data[$ver] = $link;
				
				$json = json_encode($json_data);
				if (file_put_contents($json_file_path, $json)) {
					returnJsonMsg("Success");
				} else {
					returnJsonMsg("Error", "Error writing json file...");
				}
			}
		}
		returnJsonMsg("Error", '\'version\' or \'link\' parameters not exists or empty');
	}
	returnJsonMsg("Error", 'invalid json');
} if ($_SERVER['REQUEST_METHOD'] === 'DELETE') {
	if (isset($_GET['ver'])) {
		$ver = $_GET['ver'];
		
		if (array_key_exists($ver, $json_data)) {
			unset($json_data[$ver]);
			
			$json = json_encode($json_data);
			if (file_put_contents($json_file_path, $json)) {
				returnJsonMsg("Success");
			} else {
				returnJsonMsg("Error", "Error writing json file...");
			}
		}
		returnJsonMsg("Error", 'This \'ver\' not exists');
	}
	returnJsonMsg("Error", '\'ver\' parameter not exists or empty');
} else {
	if (isset($_GET['ver'])) {
		$ver = $_GET['ver'];
		
		$ver = strtolower($ver);
		$json_data = array_change_key_case($json_data, CASE_LOWER);
		
		if (array_key_exists($ver, $json_data)) {
			redirect($json_data[$ver]);
		} else {
			logInFile($ver);
		}
	}
	if (isset($_GET['ver_all'])) {
		$data = [
			'result' => 'Success',
			'data' => $json_data
		];
		returnJsonData($data);
	}

	http_response_code(404);
	die();
}
?>
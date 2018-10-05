<?php

#Options are passed along with the post in the URL as GET parameters.
#       Use sleep to cause the server to sleep for the specified seconds before returning (max of 30).
#       Use status_code to cause the server to return a user defined status code
#       Use the "dir" parameter to specify a subdirecty for your posts. This makes it easier to find them. E.G: ?dir=henry
#       Append ?dump to the url to return the full dump of the post in the response
#       Additionally add &html to view the returned dump formatted for a browser
#       So the all-options-on url is: http://posttestserver.com/pts.php?dump&html&dir=henry&status_code=202&sleep=2
#oryginal:
# http://www.posttestserver.com/index-old.html

use google\appengine\api\cloud_storage\CloudStorageTools;

#$default_bucket = CloudStorageTools::getDefaultGoogleStorageBucketName();
$default_bucket = "php-post-storage";

#$basedir = "gs://php-post-datastore/data/";
$basedir = "gs://${default_bucket}/post/";
$filedir = "gs://${default_bucket}/post/";

date_default_timezone_set('Europe/Warsaw');
$hostname="php-post-server-195907.appspot.com";
$random=rand();

$ignore = false;
$hostname_as_filename = "domyslny";

# Enable CORS
header('Access-Control-Allow-Origin: *');

function makeDir($dir)
{
  if(file_exists($dir) == false) {
    mkdir($dir);
  }
}

function dir_with_hostname($startDir, $host_as_filename)
{
  $dir = $startDir . $host_as_filename;
  makeDir($dir);
  
  return $dir;
}

if (isset($_GET['status_code']))
{
  $status = $_GET['status_code'];
  header("HTTP/1.0 $status Custom Status", true, $status);
}

#"usypia" odpowiedz serwera na max 30 sekund
if(isset($_GET['sleep']))
{
  $sleep_count = $_GET['sleep'];
  if($sleep_count > 30) { $sleep_count = 30; }
  sleep($sleep_count);
}

$output = "Time: " . date(DATE_RFC822) . "\n";
$output .= "Source ip: " . getenv('REMOTE_ADDR') . "\n";

$output .= "\nHeaders (Some may be inserted by server)\n";
foreach ($_SERVER as $name => $content) {
  # ignore server specific content (confuses people)
  if(preg_match("/^PATH/", $name) ||
     preg_match("/^RAILS/", $name) ||
         preg_match("/^FCGI/", $name) ||
         preg_match("/^SCRIPT_URL/", $name) ||
         preg_match("/^SCRIPT_URI/", $name) ||
         preg_match("/^dsid/", $name) ||
         preg_match("/^ds_id/", $name) ||
         preg_match("/^DH_USER/", $name) ||
         preg_match("/^DOCUMENT/", $name) ||
         preg_match("/^SERVER/", $name) ||
         preg_match("/^SCRIPT/", $name) ||
         preg_match("/^argv/", $name) ||
         preg_match("/^argc/", $name) ||
         preg_match("/^PHP/", $name) ||
         preg_match("/^SCRIPT/", $name) ) {
    continue;
  }
}

$output .= "\n";

$totalsize = (int) $_SERVER['CONTENT_LENGTH'];
if($totalsize > 537387 ) {
  echo "Posted message too large. :(";
  exit;
}

if($_POST && count($_POST) > 0 )
{
  $output .= "Post Params:\n";
  foreach ($_POST as $key => $value) {
  #  if($key == "var1" && $value = "lol") {
  #    $ignore = true;
  #      }
    if($key == "Hostname") {
        $hostname_as_filename = $value;
    }      
    $output .= "key: '$key' value: '$value'\n";
  }
}
else
{
  $output .= "No Post Params.\n";
}

/*
if($HTTP_RAW_POST_DATA)
{
  $output .= "\n== Begin post body ==\n";
  $output .= $HTTP_RAW_POST_DATA;
  $output .= "\n== End post body ==\n";
}
else
{
  $output .= "Empty post body.\n";
}
 */

//TUTAJ:
//$dir = dir_with_hostname($basedir,$hostname_as_filename);
$dir = $basedir;

if(! empty($_GET) && isset($_GET["dir"])) {
  $target = str_replace(".", "", $_GET["dir"]);
  $target = str_replace("/", "", $target);
  $target = str_replace(";", "", $target);

  if(strlen($target) > 1) {
    $dir = $dir . $target;
        makeDir($dir);
  }
}


# Handle multipart/form-data
# $_FILES is a hash of hashes, one for each uploaded file
if(isset($_SERVER["CONTENT_TYPE"]) &&
   preg_match("/multipart\/form-data/i", $_SERVER["CONTENT_TYPE"] )
)
{
  $output .= "\n== Multipart File upload. ==\n";
  $output .= "Received " . count($_FILES) . " file(s)\n";
  $count = 0;
  foreach($_FILES as $key => $value)
  {
    $output .= " $count: posted name=$key\n";
        $f_name=$key;
        foreach($_FILES[$key] as $key2 => $value2)
	{
		if(!strcmp($key2, "tmp_name"))
		{
            		continue;
            	}

          	$output .= "    $key2: $value2\n";
        }

        # move the file from temp storage to the actual destination
        $uploaded = $_FILES[$key]['tmp_name'];
        $target_filename = date("Y-m-d_H.i.s") . "_" . $hostname_as_filename . "_" . $f_name . ".txt";
	$target_path = $dir . "/$target_filename";
	$target_url = "/post/".$target."/".$target_filename;

    if(is_uploaded_file($uploaded))
    {
      if(copy($uploaded, $target_path)) {
        $output .= "Uploaded File: http://$hostname$target_url\n";
          }
          else {
            $output .= "File uploaded successfully but could not be copied.\n";
          }
    }
    else {
      $output .= "File specified was not uploaded. Possible file upload attack.\n";
    }
  }
}



$putdata = fopen("php://input", "r");

$didit = false;
while ($data = fread($putdata, 1024)) {
  if(!$didit) {
    $output .= "\nUpload contains PUT data:\n";
        $didit = true;
  }
  $output .= $data;
}
fclose($putdata);


if($ignore) {
  exit;
}

#
#----------------------------------------------------------
# na google cloud:
#$default_bucket = CloudStorageTools::getDefaultGoogleStorageBucketName();
#file_put_contents("gs://${default_bucket}/hello_default.txt", $newFileContent);

#$default_bucket = CloudStorageTools::getDefaultGoogleStorageBucketName();
#$fp = fopen("gs://${default_bucket}/hello_default_stream.txt", 'w');
#fwrite($fp, $newFileContent);
#fclose($fp);

$filename = date("Y-m-d_H.i.s") . "_" . $hostname_as_filename . ".txt";
$file = $basedir . "$filename";
file_put_contents($file,$output);

#
#----------------------------------------------------------
# na linuksie:

#$filename = date("Y-m-d.H.i.s") . $random . ".txt";
#$file = $dir . "/$filename";
#$fh = fopen($file, 'w');
#fwrite($fh, $output);
#fclose($fh);

#
#----------------------------------------------------------
#


if (isset($_GET['response_body']))
{
	echo $_GET['response_body'];
}
else
{
	echo "Successfully dumped " . count($_POST) . " post variables.\n";
	$path = "";
	if(isset($_GET["dir"]))
	{
		$path .=  $_GET["dir"] . "/";
        }
	$path .= "$filename\n";

	//echo "View it at http://$hostname/post/$hostname_as_filename/$path";
        echo "View it at http://$hostname/post/$path";
	#echo "Post body was " . strlen($HTTP_RAW_POST_DATA) . " chars long.";
        echo "\n######------------ ZAWAROSC POST SERVER -------------------######\n\n";
        echo $output;
        echo "\n######------------ KONIEC -------------------######";
	
}
?>


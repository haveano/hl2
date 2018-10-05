<?php

use google\appengine\api\cloud_storage\CloudStorageTools;

if(isset($_GET['file']))
{
  $file = $_GET['file'];
  #if($sleep_count > 30) { $sleep_count = 30; }
  #sleep($sleep_count);
  echo "file: $file\n\n";
  echo "<br /><br />";
}


$my_bucket = "php-post-storage";
$filename = "gs://${my_bucket}/files/$file";

if (file_exists($filename)) {
	echo "The file $filename exists! :)\n\n<br /><br />";
	$publicUrl = CloudStorageTools::getPublicUrl($filename, false);
	echo $publicUrl;
} else {
    echo "The file $filename does not exist\n";
}

?>


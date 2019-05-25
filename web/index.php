<!doctype html>
<html>
<head>
<meta charset="utf-8">
<title>NAScrawler</title>
<meta name="viewport" content="width=device-width, initial-scale=1">
<link href="//fonts.googleapis.com/css?family=Roboto" rel="stylesheet">
<!-- Latest compiled and minified CSS -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

<!-- Optional theme -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous">


<link rel="stylesheet" type="text/css" href="style.css">

</head>

<body>
	<?php

	if(isset($_GET["q"]) && trim($_GET["q"])!=""){
		$q=filter_var($_GET["q"], FILTER_SANITIZE_STRING);
		require 'connect.php';
		require 'util.php';

		if ($conn->connect_error) {
			die("Connection failed: " . $conn->connect_error);
		}

		/* SECURE VERSION
		$stmt = $conn->prepare('SELECT * FROM files WHERE name LIKE ? ORDER BY name');
		$query="%".trim($_GET["q"])."%";
		$stmt->bind_param("s", $query);

		$stmt->execute();
		$result = $stmt->get_result();

		*/
		
		$searchTerms = explode(' ', $q);
		$searchTermBits = array();
		foreach ($searchTerms as $term) {
			$term = trim($term);
			if (!empty($term)) {
				$searchTermBits[] = "name LIKE '%$term%'";
			}
		}
		
		$result = mysqli_query($conn,"SELECT n.url as url, f.path as path, f.name as name, f.size as size, f.extension as extension FROM files f, nas n WHERE f.nas_id=n.id AND ".implode(' AND ', $searchTermBits));
		if(!$result) {
			die("Database query failed: " . mysqli_error());
		}
		
		echo '
			<form action="/" method="get">
				<div class="search-input">
				  <input type="text" name="q" value="'.$q.'">
				  <span class="highlight"></span>
				  <span class="bar"></span>
				  <label class="not-empty">Search here</label>
				  <span class="glyphicon glyphicon-search not-empty"></span>
				  <span class="glyphicon glyphicon-remove not-empty"></span>
				</div>
			</form>';
		
		
		echo '<div class="results">
			<table>';
		echo '<tr>';
		echo '<th>Name</th>';
		echo '<th>Type</th>';
		echo '<th>Size</th>';
		echo '</tr>';
		//while ($row = $result->fetch_assoc()) {
		while ($row = mysqli_fetch_array($result)) {
			echo '<tr>';
			echo '<td><a href="http://'.$row['url'].'/'.$row['path'].'">'.$row['name'].'</a></td>';
			echo '<td>'.mime_content_type('https://d1pwix07io15pr.cloudfront.net/v9dffeddd70/images/logos/header-logo.svg').'</td>';
			echo '<td>'.human_filesize($row['size']).'</td>';
			echo '</tr>';
		}
		echo '</table></div>';

			
		//$stmt->close();
	}else{
		echo '
			<form action="/" method="get">
				<div class="search-input">
				  <input type="text" name="q">
				  <span class="highlight"></span>
				  <span class="bar"></span>
				  <label>Search here</label>
				  <span class="glyphicon glyphicon-search"></span>
				  <span class="glyphicon glyphicon-remove"></span>
				</div>
			</form>';
	}
	?>
	

<script src="//code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="//cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.6/umd/popper.min.js" integrity="sha384-wHAiFfRlMFy6i5SRaxvfOCifBUQy1xHdJ/yoi7FRNXMRBu5WHdZYu1hA6ZOblgut" crossorigin="anonymous"></script>
<!-- Latest compiled and minified JavaScript -->
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
<script src="script.js"></script>
	
</body>
</html>
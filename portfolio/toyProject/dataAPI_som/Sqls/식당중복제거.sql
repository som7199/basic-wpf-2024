SELECT * FROM Restaurant
 WHERE Idx IN (
	SELECT Idx FROM (
		SELECT ROW_NUMBER() OVER (PARTITION BY [Name] ORDER BY Idx DESC ) A,
		       Idx FROM Restaurant
	     WHERE Category = '분식'
	) B
WHERE A = 1)
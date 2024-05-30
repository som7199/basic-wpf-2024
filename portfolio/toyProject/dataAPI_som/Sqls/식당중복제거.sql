-- 즐겨찾기 카테고리별 중복 제거
SELECT * 
  FROM Restaurant
 WHERE Idx IN (
	SELECT Idx 
	  FROM (SELECT ROW_NUMBER() OVER (PARTITION BY [Name] ORDER BY Idx DESC) A, Idx 
		      FROM Restaurant
	         WHERE Category = '양식'
			 ) B
     WHERE A = 1
	 )

-- 즐겨찾기 중복 제거
SELECT * 
  FROM Restaurant
 WHERE Idx IN (
	SELECT Idx 
	  FROM (SELECT ROW_NUMBER() OVER (PARTITION BY [Name] ORDER BY Idx DESC) A, Idx 
		      FROM Restaurant
			 ) B
     WHERE A = 1
	 )
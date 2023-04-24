SELECT
	"Id" ,
	"Tag",
	"ImgUrl",
	"ImgPath"
FROM
	CRDBOX
WHERE
	("ImgUrl" IS NOT NULL AND LENGTH("ImgUrl") > 0)
	{0}
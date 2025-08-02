from requests import get

def getTotalScore(token) -> int:
	url = 'https://easy-plu.knowledge-hero.com/api/plu/knowledge/user'
	
	headers = { 'authorization': f'Bearer {token}' }

	response = get(url, headers=headers)
	response.raise_for_status()

	data = response.json()
	return data['data']

def getScoresByCategory(token) -> list[tuple[str, str]]:
	url = 'https://easy-plu.knowledge-hero.com/api/plu/knowledge/user/product-groups-results'
	
	headers = { 'authorization': f'Bearer {token}' }

	response = get(url, headers=headers)
	response.raise_for_status()

	scores = []
	data = response.json()['data']['productCategories']
	for i in range(4):
		categoryData = data[i]
		scores.append((categoryData['result'], categoryData['plu_product_category']['name']))
	return scores
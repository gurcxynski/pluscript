from requests import get

def getTotalScore(token) -> str:
	url = 'https://easy-plu.knowledge-hero.com/api/plu/knowledge/user'
	
	headers = { 'authorization': f'Bearer {token}' }

	response = get(url, headers=headers)
	response.raise_for_status()

	data = response.json()
	return data['data']
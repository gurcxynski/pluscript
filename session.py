from requests import post, put

class SessionHandler:
	def __init__(self):
		self.headers = {}
		self.token = None
		self.user_id = None
	
	def initializeSession(self, token, user_id, item_count):
		url = 'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/create-new-session'

		payload = {
			'product_category_id': None,
			'count_selection': item_count,
			'user_id': user_id,
			'language_id': 20,
			'execution_type': 3,
			'execution_subtype': 0,
			'ean_active': False,
			'top_article_active': False,
			'plu_current_count': 1,
			'attribute_group_id': None,
			'new_plu': None
		}

		self.headers = { 'authorization': f'Bearer {token}' }
		self.token = token

		response = post(url, json=payload, headers=self.headers)
		response.raise_for_status()
		data = response.json()

		self.sessionID = data['data']['session_id']

	def getExecutionItems(self) -> list:
		url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{self.sessionID}/execution-items'

		response = post(url, headers=self.headers)
		response.raise_for_status()
		data = response.json()

		ret = []
		for item in data['data']['items']:
			pluobj = item['pluNumber']
			title = pluobj['title'] or next(iter(pluobj['translations'].values()))['title'] 
			ret.append([title, pluobj['id'], item['id']])
		return ret

	def startExecution(self) -> None:
		url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{self.sessionID}/start-execution'

		response = post(url, headers=self.headers)
		response.raise_for_status()

	def sendAnswer(self, task_id, plu_number, plu_id) -> None:
		url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{task_id}/update'

		payload = {
			'execution_type': 3,
			'given_plu_number': plu_number,
			'plu_number_id': plu_id,
			'answer': {'correct': True}
		}

		response = put(url, json=payload, headers=self.headers)
		response.raise_for_status()

	def getResult(self) -> tuple[int, int]:
		url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{self.sessionID}/result'

		payload = { 'user_id': self.user_id }

		response = post(url, json=payload, headers=self.headers)
		response.raise_for_status()
		data = response.json()

		result = data['data']['result']['total_user_points']
		max_result = data['data']['result']['max_points']

		return result, max_result
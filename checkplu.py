from requests import post

def get_plu_number(token, phrase):
    url = 'https://easy-plu.knowledge-hero.com/api/plu/product/search'

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = {
        "page": 1,
        "perPage": 1,
        "search": phrase,
    }

    response = post(url, json=payload, headers=headers)
    data = response.json()
    return data["data"]["products"]["data"][0]["plu_number"]

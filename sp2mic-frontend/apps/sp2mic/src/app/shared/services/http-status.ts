export const HTTP_STATUS = {
  OK_200: 200,
  CREATED_201: 201,
  ACCEPTED_202: 202,
  NO_CONTENT_204: 204,

  NOT_MODIFIED_304: 304,

  BAD_REQUEST_400: 400,
  UNAUTHORIZED_401: 401,
  FORBIDDEN_403: 403,
  NOT_FOUND_404: 404,
  REQUEST_TIMEOUT_408: 408,
  CONFLICT_409: 409,
  UNPROCESSABLE_ENTITY_422: 422,

  INTERNAL_SERVER_ERROR_500: 500,
  SERVICE_UNAVAILABLE_503: 503,
  PERMISSION_DENIED_550: 550,
  GATEWAY_TIMEOUT_504: 504,

  //isInformational: (status: number) => status >= 100 && status < 200,
  //isSuccessful: (status: number) => status >= 200 && status < 300,
  //isRedirection: (status: number) => status >= 300 && status < 400,
  //isClientError: (status: number) => status >= 400 && status < 500,
  //isServerError: (status: number) => status >= 500 && status < 600
};

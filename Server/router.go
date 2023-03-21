package main

import (
	"github.com/labstack/echo/v4"
)

func ConnectEndpoints(e *echo.Echo) {
	e.GET("/", mainPage)

	e.GET("/list", List)
	e.POST("/connect/:session", Connect)

	e.POST("/stream", connectServerEcho)
}

//func websocketWrapper(function func(ws *websocket.Conn) error) func(c echo.Context) error {
//	return func(c echo.Context) error {
//		ws, err := upgrader.Upgrade(c.Response(), c.Request(), nil)
//		if err != nil {
//			return err
//		}
//		err = function(ws)
//		if err != nil {
//			log.WithField("websocket", c.Path()).Debug(err)
//		}
//		return err
//	}
//}

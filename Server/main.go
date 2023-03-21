package main

import (
	"context"
	"flag"
	"fmt"
	"github.com/labstack/echo/v4/middleware"
	"math/rand"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/sirupsen/logrus"
)

var (
	log         *logrus.Logger
	multiLogger *logrus.Entry
)

var (
	port     = flag.Int("port", 37892, "port number for server")
	mClients = flag.Int("maxPlayersPerSession", 2, "port number for server")

	addr       string
	maxClients int
)

func init() {
	flag.Parse()
	addr = fmt.Sprintf(":%d", *port)
	maxClients = *mClients

	log = logrus.New()
	multiLogger = log.WithField("mode", "multi server")
	log.Level = logrus.DebugLevel
}

func main() {
	e := echo.New()
	e.HideBanner = true
	echo.NotFoundHandler = func(c echo.Context) error {
		return c.String(http.StatusNotFound, "")
	}

	e.Use(middleware.RequestLoggerWithConfig(middleware.RequestLoggerConfig{
		LogURI:    true,
		LogStatus: true,
		LogValuesFunc: func(c echo.Context, values middleware.RequestLoggerValues) error {
			log.WithFields(logrus.Fields{
				"URI":    values.URI,
				"status": values.Status,
			}).Info("request")

			return nil
		},
	}))
	e.Use(middleware.Recover())

	ConnectEndpoints(e)

	rand.Seed(time.Now().Unix())
	server := NewGameServer()

	go func() {
		if err := e.Start(addr); err != nil && err != http.ErrServerClosed {
			e.Logger.Fatal("shutting down the server")
		}
	}()

	signalChan := make(chan os.Signal, 1)
	signal.Notify(signalChan, os.Interrupt, syscall.SIGINT, syscall.SIGTERM)
	<-signalChan
	log.Println("Shutting down")

	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	server.Stop()
	if err := e.Shutdown(ctx); err != nil {
		e.Logger.Fatal(err)
	}
}

func mainPage(e echo.Context) error {
	e.Response().Header().Set("game", "artemis-falling-server")
	return e.String(http.StatusNoContent, "idk how you got here, this is a game server")
}

import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { AddVideo } from './components/AddVideo';
import { AddChannel } from './components/AddChannel';
import { Search } from './components/Search';

export default function App() {
  return (
    <Layout>
      <Route exact path='/' component={Home} />
      <Route path='/add-video' component={AddVideo} />
      <Route path='/add-channel' component={AddChannel} />
      <Route path='/search' component={Search} />
    </Layout>
  );
}

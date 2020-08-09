import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { AddVideo } from './components/AddVideo';
import { AddChannel } from './components/AddChannel';
import { Search } from './components/Search';

export default function App() {
  return (
    <Layout>
      <Route path='/add-video' component={AddVideo} />
      <Route path='/add-channel' component={AddChannel} />
      <Route path='/search' component={Search} />
    </Layout>
  );
}
